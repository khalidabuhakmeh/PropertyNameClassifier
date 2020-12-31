using System;
using System.IO;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace Classifier
{
    public static class MachineLearning
    {
        private const string ModelPath = "property_names.model";

        public static PredictionEngine<PropertyName, PropertyTypePrediction> LoadExistingModel(MLContext context)
        {
            if (File.Exists(ModelPath))
            {
                var loadedModel = context.Model.Load(ModelPath, out var modelInputSchema);
                return context.Model.CreatePredictionEngine<PropertyName, PropertyTypePrediction>(loadedModel);
            }
            else
            {
                return TrainAndSaveModel(context);
            }
        }

        public static PredictionEngine<PropertyName, PropertyTypePrediction> TrainAndSaveModel(MLContext context)
        {
            var data = context.Data.LoadFromTextFile<PropertyName>(
                "data.csv", new TextLoader.Options()
                {
                    HasHeader = false,
                    TrimWhitespace = true,
                    Separators = new[] {','}
                });

            var pipeline = context
                .Transforms
                .Conversion
                .MapValueToKey(
                    inputColumnName: nameof(PropertyName.Kind),
                    outputColumnName: "Label"
                )
                .Append(
                    context.Transforms.Text.FeaturizeText(
                        inputColumnName: nameof(PropertyName.Name),
                        outputColumnName: "NameFeaturized"
                    )
                )
                .Append(
                    context.Transforms.Concatenate("Features", "NameFeaturized")
                );

            var trainingPipeline = pipeline
                .Append(context.MulticlassClassification.Trainers.SdcaMaximumEntropy())
                .Append(context.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

            var trainedModel = trainingPipeline.Fit(data);
            var engine = context.Model.CreatePredictionEngine<PropertyName, PropertyTypePrediction>(trainedModel);

            var testMetrics = context.MulticlassClassification.Evaluate(trainedModel.Transform(data));

            Console.WriteLine($"*************************************************************************************************************");
            Console.WriteLine($"*       Metrics for Multi-class Classification model - Test Data     ");
            Console.WriteLine($"*------------------------------------------------------------------------------------------------------------");
            Console.WriteLine($"*       MicroAccuracy:    {testMetrics.MicroAccuracy:0.###}");
            Console.WriteLine($"*       MacroAccuracy:    {testMetrics.MacroAccuracy:0.###}");
            Console.WriteLine($"*       LogLoss:          {testMetrics.LogLoss:#.###}");
            Console.WriteLine($"*       LogLossReduction: {testMetrics.LogLossReduction:#.###}");
            Console.WriteLine($"*************************************************************************************************************");

            context.Model.Save(trainedModel, data.Schema, ModelPath);
            return engine;
        }
    }
}
using System;
using Classifier;
using Microsoft.ML;

var context = new MLContext(1);
// to re-train the model call "MachineLearning.TrainAndSaveModel"
// or clean the output directory
var engine =  MachineLearning.LoadExistingModel(context);

var input = new PropertyName {Name = "Id"};
var result = engine.Predict(input);

Console.WriteLine();
Console.WriteLine($"Predicting for \"{input.Name}\": {result.Kind} ");
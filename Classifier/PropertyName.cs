using Microsoft.ML.Data;

public class PropertyName
{
    [LoadColumn(0)]
    public string Name { get; set; }
    
    [LoadColumn(1)]
    public string Kind { get; set; }
}
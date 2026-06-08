namespace Documents.EndPoints.Models;

public record ManualRecognition
{
    public required long DocumentId { get; set; }

    public required string Label { get; set; }

    public required float Probability { get; set; }
}

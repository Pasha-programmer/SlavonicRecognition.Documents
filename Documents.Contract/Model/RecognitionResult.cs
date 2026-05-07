namespace Documents.Contract.Model;

public record RecognitionResult
{
    public required long DocumentId { get; set; }

    public required string Label { get; set; }

    public required float Probability { get; set; }
}

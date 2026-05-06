namespace Documents.Contract.Model;

public record RecognitionResult
{
    public required int DocumentId { get; set; }

    public required string Label { get; set; }

    public required float Probability { get; set; }
}

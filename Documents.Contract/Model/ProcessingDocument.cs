namespace Documents.Contract.Model;

public record ProcessingDocument
{
    public required long DocumentId { get; set; }

    public required Memory<byte> Blob { get; set; }

    public required AiModelType AiModelType { get; set; }
}

namespace Documents.Contract.Model;

public record RecognizedDocumentDto
{
    public required long DocumentId { get; set; }

    public required string FileName { get; set; }

    public required ReadOnlyMemory<byte> FileBlob { get; set; }

    public required string Content { get; set; }
}

namespace Documents.Contract.Model.Document;

public record DocumentDto
{
    public required long DocumentId { get; set; }

    public required byte[] FileBlob { get; set; }
}

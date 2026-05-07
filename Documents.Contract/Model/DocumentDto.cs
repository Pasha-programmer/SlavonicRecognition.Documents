namespace Documents.Contract.Model;

public record DocumentDto
{
    public required long DocumentId { get; set; }

    public required byte[] FileBlob { get; set; }
}

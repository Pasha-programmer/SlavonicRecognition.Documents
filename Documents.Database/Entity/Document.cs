namespace Documents.Database.Entity;

public class Document
{
    public long Id { get; set; }

    public string FileName { get; set; }

    public DateTime CreateAt { get; set; }

    public byte[] FileBlob { get; set; }
}

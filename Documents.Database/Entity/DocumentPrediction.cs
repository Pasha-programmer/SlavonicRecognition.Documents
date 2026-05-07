namespace Documents.Database.Entity;

public class DocumentPrediction
{
    public int Id { get; set; }

    public long DocumentId { get; set; }

    public required string Value { get; set; }

    public float Prob { get; set; }
}

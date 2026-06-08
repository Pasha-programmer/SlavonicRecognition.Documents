namespace Documents.Database.Entity;

public class DocumentPrediction
{
    public long Id { get; set; }

    public long DocumentId { get; set; }

    public int RecognitionType { get; set; }

    public int? ModelType { get; set; }

    public required string Value { get; set; }

    public float Prob { get; set; }
}

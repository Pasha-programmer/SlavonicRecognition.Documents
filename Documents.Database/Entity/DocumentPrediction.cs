namespace Documents.Database.Entity;

public class DocumentPrediction
{
    public int Id { get; set; }

    public int DocumentId { get; set; }

    public required string Value { get; set; }

    public double Prob { get; set; }
}

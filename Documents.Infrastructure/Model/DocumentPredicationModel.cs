namespace Documents.Infrastructure.Model;

internal record DocumentPredicationModel
{
    public long DocumentId { get; set; }

    public string FileName { get; set; }

    public byte[] FileBlob { get; set; }

    public int SelectedModelType { get; set; }

    public DateTime CreateAt { get; set; }

    public long? DocumentPredictionId { get; set; }

    public int? ModelType { get; set; }

    public int? RecognitionType { get; set; }

    public string? Label { get; set; }

    public float? Probability { get; set; }
}

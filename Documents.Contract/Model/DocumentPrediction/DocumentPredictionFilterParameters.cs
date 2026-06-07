using Documents.Contract.Model.Enums.AiModel;

namespace Documents.Contract.Model.DocumentPrediction;

public record DocumentPredictionFilterParameters
{
    public long[]? DocumentPredictionIds { get; set; }

    public RecognitionType[]? RecognitionTypes { get; set; }

    public AiModelType[]? ModelTypes { get; set; }

    public DateTime? FromDate { get; set; }

    public DateTime? ToDate { get; set; }

    public bool? HasProbability { get; set; }
}

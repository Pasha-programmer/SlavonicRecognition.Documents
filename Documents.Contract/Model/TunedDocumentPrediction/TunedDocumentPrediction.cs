using Documents.Contract.Model.Enums.AiModel;

namespace Documents.Contract.Model.TunedDocumentPrediction;

public record TunedPrediction
{
    public long Id { get; set; }

    public long DocumentPredictionId { get; set; }

    public AiModelType ModelType { get; set; }
}

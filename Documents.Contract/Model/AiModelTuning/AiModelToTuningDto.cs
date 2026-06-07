using Documents.Contract.Model.Enums.AiModel;

namespace Documents.Contract.Model.AiModelTuning;

public record AiModelToTuningDto
{
    public required AiModelType AiModelType { get; set; }

    public required long[] DocumentPredictionIds { get; set; }
}

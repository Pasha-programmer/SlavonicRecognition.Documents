using Documents.Contract.Model.Enums.AiModel;

namespace Documents.Infrastructure.Model;

public record AiModelToTuningModel
{
    public required long DocumentPredictionId { get; set; }

    public required AiModelType AiModelType { get; set; }

    public required ReadOnlyMemory<byte> FileBlob { get; set; }

    public required string Label { get; set; }
}

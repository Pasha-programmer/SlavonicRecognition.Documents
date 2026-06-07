using Documents.Contract.Model.Enums.AiModel;

namespace Documents.Contract.Model.DocumentPrediction;

public record RecognitionResultDto
{
    public long? Id { get; set; }

    public required long DocumentId { get; set; }

    public required AiModelType? ModelType { get; set; }

    public required RecognitionType RecognitionType { get; set; }

    public required string Label { get; set; }

    public required float Probability { get; set; }
}

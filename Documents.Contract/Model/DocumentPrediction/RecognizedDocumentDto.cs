using Documents.Contract.Model.Enums.AiModel;

namespace Documents.Contract.Model.DocumentPrediction;

public record RecognizedDocumentDto
{
    public required long DocumentId { get; set; }

    public required string FileName { get; set; }

    public required ReadOnlyMemory<byte> FileBlob { get; set; }

    public required AiModelType SelectedModelType { get; set; }

    public required IReadOnlyCollection<RecognitionResultDto> RecognitionResults { get; set; }
}

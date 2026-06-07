using Documents.Contract.Model.DocumentPrediction;

namespace Documents.Contract.DocumentPrediction;

public interface IDocumentPredictionCommandService
{
    Task<bool> AddPredication(IReadOnlyCollection<RecognitionResultDto> recognitionResult, CancellationToken cancellationToken = default);
}

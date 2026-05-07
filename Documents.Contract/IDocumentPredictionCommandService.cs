using Documents.Contract.Model;

namespace Documents.Contract;

public interface IDocumentPredictionCommandService
{
    Task<bool> AddPredication(IReadOnlyCollection<RecognitionResult> recognitionResult, CancellationToken cancellationToken = default);
}

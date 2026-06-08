using Documents.Contract.Model.DocumentPrediction;

namespace Documents.Contract.DocumentPrediction;

public interface IDocumentPredictionCommandService
{
    Task<bool> AddPredication(IReadOnlyCollection<RecognitionResultDto> recognitionResult, CancellationToken cancellationToken = default);

    Task<bool> UpdatePredication(IReadOnlyCollection<RecognitionResultDto> recognitionResult, CancellationToken cancellationToken = default);

    Task<bool> DeletePredications(IReadOnlyCollection<long> documentPredictionIds, CancellationToken cancellationToken = default);
}

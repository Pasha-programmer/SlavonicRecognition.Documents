using Documents.Contract.Model;

namespace Documents.Contract;

public interface IDocumentPredictionQueryService
{
    Task<IReadOnlyCollection<RecognizedDocumentDto>> GetFilePredications(
        DateTime? fromDate, 
        DateTime? toDate, 
        bool? hasProbability, 
        CancellationToken cancellationToken = default);

    Task<bool> AddPredication(IReadOnlyCollection<RecognitionResult> recognitionResult, CancellationToken cancellationToken = default);
}

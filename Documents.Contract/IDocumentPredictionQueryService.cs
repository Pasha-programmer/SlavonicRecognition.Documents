using Documents.Contract.Model;

namespace Documents.Contract;

public interface IDocumentPredictionQueryService
{
    Task<IReadOnlyCollection<RecognizedDocumentDto>> GetFilePredications(DateTime? fromDate, DateTime? toDate, CancellationToken cancellationToken = default);

    Task<bool> AddPredication(RecognitionResult recognitionResult, CancellationToken cancellationToken = default);
}

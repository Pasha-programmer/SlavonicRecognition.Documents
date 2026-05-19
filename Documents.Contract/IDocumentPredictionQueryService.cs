using Documents.Contract.Model;

namespace Documents.Contract;

public interface IDocumentPredictionQueryService
{
    Task<IReadOnlyCollection<RecognizedDocumentDto>> GetFilePredications(
        DateTime? fromDate, 
        DateTime? toDate, 
        bool? hasProbability, 
        CancellationToken cancellationToken = default);

    ValueTask<IReadOnlyDictionary<AiModelType, double>> GetAiModelTestAccuracy();
}

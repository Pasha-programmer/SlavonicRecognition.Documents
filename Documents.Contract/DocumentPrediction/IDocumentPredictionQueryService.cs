using Documents.Contract.Model.DocumentPrediction;
using Documents.Contract.Model.Enums.AiModel;

namespace Documents.Contract.DocumentPrediction;

public interface IDocumentPredictionQueryService
{
    Task<IReadOnlyCollection<RecognizedDocumentDto>> GetDocumentPredications(
        DocumentPredictionFilterParameters? filterParameters,
        CancellationToken cancellationToken = default);

    ValueTask<IReadOnlyDictionary<AiModelType, double>> GetAiModelTestAccuracy();
}

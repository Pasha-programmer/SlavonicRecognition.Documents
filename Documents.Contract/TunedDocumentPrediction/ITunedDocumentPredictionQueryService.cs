using Documents.Contract.Model.Enums.AiModel;
using Documents.Contract.Model.TunedDocumentPrediction;

namespace Documents.Contract.TunedDocumentPrediction;

public interface ITunedDocumentPredictionQueryService
{
    Task<IReadOnlyCollection<TunedPrediction>> GetTunedDocumentPredictions(
        IReadOnlyCollection<long>? documentPredictionIds,
        IReadOnlyCollection<AiModelType>? aiModelTypes, 
        CancellationToken cancellationToken = default);
}

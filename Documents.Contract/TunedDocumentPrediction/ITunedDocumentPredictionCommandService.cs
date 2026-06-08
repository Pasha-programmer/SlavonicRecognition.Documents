using Documents.Contract.Model.TunedDocumentPrediction;

namespace Documents.Contract.TunedDocumentPrediction;

public interface ITunedDocumentPredictionCommandService
{
    Task<bool> AddTunedPredication(IReadOnlyCollection<TunedPrediction> tunedPrediction, CancellationToken cancellationToken = default);
}

using Documents.Contract.Model.Enums.AiModel;
using Documents.Contract.Model.TunedDocumentPrediction;
using Documents.Contract.TunedDocumentPrediction;
using Documents.Database;
using Microsoft.EntityFrameworkCore;

namespace Documents.Infrastructure.TunedDocumentPrediction;

public class TunedDocumentPredictionQueryService(
    IDbContextFactory<DocumentContext> contextFactory
    ) : ITunedDocumentPredictionQueryService
{
    private readonly IDbContextFactory<DocumentContext> _contextFactory = contextFactory;

    /// <inheritdoc/>
    public async Task<IReadOnlyCollection<TunedPrediction>> GetTunedDocumentPredictions(
        IReadOnlyCollection<long>? documentPredictionIds,
        IReadOnlyCollection<AiModelType>? aiModelTypes,
        CancellationToken cancellationToken = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        var query = from tdp in context.TunedDocumentPredictions
                    select new
                    {
                        Id = tdp.Id,
                        ModelType = tdp.ModelType,
                        DocumentPredictionId = tdp.DocumentPredictionId,
                    };

        if (documentPredictionIds?.Count > 0)
        {
            query = query.Where(x => documentPredictionIds.Contains(x.DocumentPredictionId));
        }

        if (aiModelTypes?.Count > 0)
        {
            var castedAiModelTypes = aiModelTypes.Cast<int>();
            query = query.Where(x => castedAiModelTypes.Contains(x.ModelType));
        }

        var data = await query.ToArrayAsync(cancellationToken);

        return data.Select(tdp => new TunedPrediction
        {
            Id = tdp.Id,
            ModelType = (AiModelType)tdp.ModelType,
            DocumentPredictionId = tdp.DocumentPredictionId,
        }).ToArray();
    }
}

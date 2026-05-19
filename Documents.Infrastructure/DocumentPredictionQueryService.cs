using Documents.Contract;
using Documents.Contract.Model;
using Documents.Database;
using Microsoft.EntityFrameworkCore;

namespace Documents.Infrastructure;

internal class DocumentPredictionQueryService(
    IDbContextFactory<DocumentContext> contextFactory) 
    : IDocumentPredictionQueryService
{
    private readonly IReadOnlyDictionary<AiModelType, double> _modelTypeAccuracyMap = new Dictionary<AiModelType, double>
    {
        { AiModelType.GlagoliticModelFullV1_1,  0.7845 },
        { AiModelType.GlagoliticModelFullV2_0,  0.7958 },
        { AiModelType.GlagoliticModelFullV2_1,  0.9324 },
        { AiModelType.GlagoliticModelFullV2_2,  0.8817 },
        { AiModelType.GlagoliticModelFullV3_0,  0.9688 },
        { AiModelType.GlagoliticModelFullV4_0,  0.9688 },
    };

    public ValueTask<IReadOnlyDictionary<AiModelType, double>> GetAiModelTestAccuracy()
    {
        return ValueTask.FromResult(_modelTypeAccuracyMap);
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyCollection<RecognizedDocumentDto>> GetFilePredications(
        DateTime? fromDate, 
        DateTime? toDate,
        bool? hasProbability,
        CancellationToken cancellationToken = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);

        var query = from d in context.Documents
                    join dp in context.DocumentPredictions on d.Id equals dp.DocumentId into dp0
                    from dp in dp0.DefaultIfEmpty()

                    select new
                    {
                        DocumentId = d.Id,
                        FileName = d.FileName,
                        FileBlob = d.FileBlob,
                        SelectedModelType = d.SelectedModelType,
                        ModelType = dp != null ? dp.ModelType : (int?)null,
                        Label = dp != null ? dp.Value : null,
                        CreateAt = d.CreateAt,
                        Probability = dp != null ? dp.Prob : (float?)null,
                    };

        if (fromDate.HasValue)
            query = query.Where(x => x.CreateAt >= fromDate);

        if (toDate.HasValue)
            query = query.Where(x => x.CreateAt < toDate);

        if (hasProbability.HasValue)
            query = query.Where(x => x.Probability.HasValue == hasProbability.Value);

        var data = await query.ToArrayAsync(cancellationToken);

        return data.GroupBy(d => d.DocumentId)
            .Select(gd =>
            {
                var firstItem = gd.First();
                return new RecognizedDocumentDto
                {
                    DocumentId = gd.Key,
                    FileBlob = firstItem.FileBlob,
                    FileName = firstItem.FileName,
                    SelectedModelType = (AiModelType)firstItem.SelectedModelType,
                    RecognitionResults = gd.Where(d => d.Label != null)
                        .Select(d => new RecognitionResult
                        {
                            DocumentId = gd.Key,
                            ModelType = (AiModelType)d.ModelType!,
                            Label = d.Label,
                            Probability = d.Probability!.Value,
                        }).ToArray(),
                };
            }).ToArray();
    }
}

using Documents.Contract.DocumentPrediction;
using Documents.Contract.Model.DocumentPrediction;
using Documents.Contract.Model.Enums.AiModel;
using Documents.Database;
using Documents.Infrastructure.Model;
using Microsoft.EntityFrameworkCore;

namespace Documents.Infrastructure.DocumentPrediction;

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
    public async Task<IReadOnlyCollection<RecognizedDocumentDto>> GetDocumentPredications(
        DocumentPredictionFilterParameters? filterParameters,
        CancellationToken cancellationToken = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);

        var query = from d in context.Documents
                    join dp in context.DocumentPredictions on d.Id equals dp.DocumentId into dp0
                    from dp in dp0.DefaultIfEmpty()

                    select new DocumentPredicationModel
                    {
                        DocumentId = d.Id,
                        FileName = d.FileName,
                        FileBlob = d.FileBlob,
                        SelectedModelType = d.SelectedModelType,
                        CreateAt = d.CreateAt,

                        DocumentPredictionId = dp != null ? dp.Id : (int?)null,
                        ModelType = dp != null ? dp.ModelType : null,
                        RecognitionType = dp != null ? dp.RecognitionType : (int?)null,
                        Label = dp != null ? dp.Value : null,
                        Probability = dp != null ? dp.Prob : (float?)null,
                    };

        query = ApplyFilterParameters(query, filterParameters);

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
                        .Select(d => new RecognitionResultDto
                        {
                            Id = gd.Key,
                            DocumentId = gd.Key,
                            ModelType = (AiModelType?)d.ModelType,
                            RecognitionType = (RecognitionType)d.RecognitionType!,
                            Label = d.Label!,
                            Probability = d.Probability!.Value,
                        }).ToArray(),
                };
            }).ToArray();
    }

    private IQueryable<DocumentPredicationModel> ApplyFilterParameters(IQueryable<DocumentPredicationModel> query, DocumentPredictionFilterParameters? filterParameters)
    {
        if (filterParameters == null)
            return query;
            
        if (filterParameters.DocumentPredictionIds?.Length > 0)
            query = query.Where(x => x.DocumentPredictionId.HasValue && filterParameters.DocumentPredictionIds.Contains(x.DocumentPredictionId.Value));

        if (filterParameters.RecognitionTypes?.Length > 0)
        {
            var castedRecognitionTypes = filterParameters.RecognitionTypes.Cast<int>();
            query = query.Where(x => x.RecognitionType.HasValue && castedRecognitionTypes.Contains(x.RecognitionType.Value));
        }

        if (filterParameters.ModelTypes?.Length > 0)
        {
            var castedModelTypes = filterParameters.ModelTypes.Cast<int>();
            query = query.Where(x => x.ModelType.HasValue && castedModelTypes.Contains(x.ModelType.Value));
        }

        if (filterParameters.FromDate.HasValue)
            query = query.Where(x => x.CreateAt >= filterParameters.FromDate);

        if (filterParameters.ToDate.HasValue)
            query = query.Where(x => x.CreateAt < filterParameters.ToDate);

        if (filterParameters.HasProbability.HasValue)
            query = query.Where(x => x.Probability.HasValue == filterParameters.HasProbability.Value);

        return query;
    }
}

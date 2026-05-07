using Documents.Contract;
using Documents.Contract.Model;
using Documents.Database;
using Microsoft.EntityFrameworkCore;

namespace Documents.Infrastructure;

internal class DocumentPredictionQueryService(
    IDbContextFactory<DocumentContext> contextFactory) 
    : IDocumentPredictionQueryService
{
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
                    RecognitionResults = gd.Where(d => d.Label != null)
                        .Select(d => new RecognitionResult
                        {
                            DocumentId = gd.Key,
                            Label = d.Label,
                            Probability = d.Probability!.Value,
                        }).ToArray(),
                };
            }).ToArray();
    }
}

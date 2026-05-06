using Documents.Contract;
using Documents.Contract.Model;
using Documents.Database;
using Documents.Database.Entity;
using Microsoft.EntityFrameworkCore;

namespace Documents.Infrastructure;

internal class DocumentPredictionQueryService(
    IDbContextFactory<DocumentContext> contextFactory) 
    : IDocumentPredictionQueryService
{
    /// <inheritdoc/>
    public async Task<IReadOnlyCollection<RecognizedDocumentDto>> GetFilePredications(DateTime? fromDate, DateTime? toDate, CancellationToken cancellationToken = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);

        var query = from d in context.Documents
                    join dp in context.DocumentPredictions on d.Id equals dp.DocumentId

                    select new
                    {
                        DocumentId = d.Id,
                        FileName = d.FileName,
                        FileBlob = d.FileBlob,
                        Content = dp.Value,
                        CreateAt = d.CreateAt,
                    };

        if (fromDate.HasValue)
            query = query.Where(x => x.CreateAt >= fromDate);

        if (toDate.HasValue)
            query = query.Where(x => x.CreateAt < toDate);

        return await query.Select(x => new RecognizedDocumentDto
        {
            DocumentId = x.DocumentId,
            FileName = x.FileName,
            FileBlob = x.FileBlob,
            Content = x.Content,
        }).ToArrayAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<bool> AddPredication(RecognitionResult recognitionResult, CancellationToken cancellationToken = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);

        context.Attach(new DocumentPrediction
        {
            DocumentId = recognitionResult.DocumentId,
            Value = recognitionResult.Label,
            Prob = recognitionResult.Probability,
        });

        return (await context.SaveChangesAsync(cancellationToken)) > 0;
    }
}

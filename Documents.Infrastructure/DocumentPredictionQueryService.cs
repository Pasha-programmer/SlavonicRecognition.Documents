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
    public async Task<IReadOnlyCollection<RecognizedDocumentDto>> GetFilePredications(DateTime? fromDate, DateTime? toDate)
    {
        await using var context = await contextFactory.CreateDbContextAsync();

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
        }).ToArrayAsync();
    }
}

using Documents.Contract;
using Documents.Contract.Model;
using Documents.Database;
using Microsoft.EntityFrameworkCore;

namespace Documents.Infrastructure;

internal class DocumentQueryService(
    IDbContextFactory<DocumentContext> contextFactory)
    : IDocumentQueryService
{
    /// <inheritdoc/>
    public async Task<IReadOnlyCollection<DocumentDto?>> GetDocuments(IReadOnlyCollection<long>? documentIds, CancellationToken cancellationToken = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);

        var query = from d in context.Documents

                    select new DocumentDto
                    {
                        DocumentId = d.Id,
                        FileBlob = d.FileBlob,
                    };

        if (documentIds != null)
        {
            query = query.Where(d => documentIds.Contains(d.DocumentId));
        }

        return await query.ToArrayAsync(cancellationToken);
    }
}

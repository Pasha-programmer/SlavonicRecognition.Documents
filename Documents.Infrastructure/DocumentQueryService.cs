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
    public async Task<DocumentDto?> GetDocument(long documentId, CancellationToken cancellationToken = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);

        var query = from d in context.Documents

                    where d.Id == documentId

                    select new DocumentDto
                    {
                        DocumentId = documentId,
                        FileBlob = d.FileBlob,
                    };

        return await query.FirstOrDefaultAsync(cancellationToken);
    }
}

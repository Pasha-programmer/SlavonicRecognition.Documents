using Documents.Contract;
using Documents.Contract.Model;
using Documents.Database;
using Documents.Database.Entity;
using Microsoft.EntityFrameworkCore;

namespace Documents.Infrastructure;

internal class DocumentCommandService(
    IDbContextFactory<DocumentContext> contextFactory)
    : IDocumentCommandService
{
    /// <inheritdoc/>
    public async Task<long> AddDocument(DocumentToCreate model, CancellationToken cancellationToken = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);

        var document = new Document
        {
            FileName = model.FileName,
            FileBlob = model.FileBlob,
            CreateAt = DateTime.Now,
        };

        var entity = await context.AddAsync(document, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        return entity.Entity.Id;
    }

    public async Task<bool> DeleteDocuments(IReadOnlyCollection<long> documentIds, CancellationToken cancellationToken = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);

        var query = from d in context.Documents
                    where documentIds.Contains(d.Id)
                    select d;

        var count = await query.ExecuteDeleteAsync(cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        return count > 0;
    }
}

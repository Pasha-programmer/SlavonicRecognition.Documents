using Documents.Contract.Document;
using Documents.Contract.Model.Document;
using Documents.Database;
using Microsoft.EntityFrameworkCore;

namespace Documents.Infrastructure.Document;

internal class DocumentCommandService(
    IDbContextFactory<DocumentContext> contextFactory)
    : IDocumentCommandService
{
    /// <inheritdoc/>
    public async Task<long> AddDocument(DocumentToCreateDto model, CancellationToken cancellationToken = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);

        var document = new Database.Entity.Document
        {
            FileName = model.FileName,
            FileBlob = model.FileBlob,
            CreateAt = DateTime.Now,
            SelectedModelType = (int)model.SelectedModelType,
        };

        var entity = await context.AddAsync(document, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        return entity.Entity.Id;
    }

    /// <inheritdoc/>
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

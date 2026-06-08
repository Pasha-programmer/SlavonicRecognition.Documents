using Documents.Contract.DocumentPrediction;
using Documents.Contract.Model.DocumentPrediction;
using Documents.Database;
using Microsoft.EntityFrameworkCore;

namespace Documents.Infrastructure.DocumentPrediction;

internal class DocumentPredictionCommandService(
    IDbContextFactory<DocumentContext> contextFactory)
    : IDocumentPredictionCommandService
{
    private readonly IDbContextFactory<DocumentContext> _contextFactory = contextFactory;

    /// <inheritdoc/>
    public async Task<bool> AddPredication(IReadOnlyCollection<RecognitionResultDto> recognitionResult, CancellationToken cancellationToken = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);

        context.AttachRange(recognitionResult.Select(rr => new Database.Entity.DocumentPrediction
        {
            DocumentId = rr.DocumentId,
            ModelType = (int?)rr.ModelType,
            RecognitionType = (int)rr.RecognitionType,
            Value = rr.Label,
            Prob = rr.Probability,
        }));

        return await context.SaveChangesAsync(cancellationToken) > 0;
    }

    /// <inheritdoc/>
    public async Task<bool> UpdatePredication(IReadOnlyCollection<RecognitionResultDto> recognitionResult, CancellationToken cancellationToken = default)
    {
        if (recognitionResult.Any(rr => !rr.Id.HasValue))
        {
            return false;
        }

        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);

        var entities = recognitionResult.Select(rr => new Database.Entity.DocumentPrediction
        {
            Id = rr.Id!.Value,
            DocumentId = rr.DocumentId,
            ModelType = (int?)rr.ModelType,
            RecognitionType = (int)rr.RecognitionType,
            Value = rr.Label,
            Prob = rr.Probability,
        });

        foreach(var entity in entities)
        {
            var entry = context.Entry(entity);

            entry.Property(e => e.Value).IsModified = true;
            entry.Property(e => e.Prob).IsModified = true;
        }

        return await context.SaveChangesAsync(cancellationToken) > 0;
    }

    /// <inheritdoc/>
    public async Task<bool> DeletePredications(IReadOnlyCollection<long> documentPredictionIds, CancellationToken cancellationToken = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);

        var query = from d in context.Documents
                    where documentPredictionIds.Contains(d.Id)
                    select d;

        var count = await query.ExecuteDeleteAsync(cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        return count > 0;
    }
}

using Documents.Contract;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Documents.EndPoints;

public static class EndPopint
{
    public static IEndpointRouteBuilder AddEndPoints(this IEndpointRouteBuilder endPoints)
    {
        var apiEndPoint = endPoints
            .MapGroup("/api");

        apiEndPoint.MapHealthChecks("/health");

        var documentEndPoints = apiEndPoint
            .MapGroup($"/documents");

        documentEndPoints.MapPost("/upload", async (
            [FromForm] IFormFileCollection images,
            [FromServices] IDocumentCommandService documentCommandService,
            [FromServices] IProcessDocument processDocument,
            CancellationToken cancellationToken) =>
        {
            if (images.Count == 0)
            {
                return Results.BadRequest("Файлы не обнаружены.");
            }

            var documentIds = new List<long>(images.Count);

            foreach (var formFile in images)
            {
                var directoryPath = Path.GetTempPath();
                var filePathUri = new Uri(Path.Combine(directoryPath, Path.GetTempFileName()));

                await using var stream = formFile.OpenReadStream();
                await using var fileStream = new FileStream(filePathUri.LocalPath, FileMode.Create);
                await stream.CopyToAsync(fileStream, cancellationToken);

                var buffer = new Memory<byte>();
                await fileStream.ReadExactlyAsync(buffer, cancellationToken);

                var documentId = await documentCommandService.AddDocument(new()
                {
                    FileName = formFile.FileName,
                    FileBlob = buffer.ToArray(),
                }, cancellationToken);

                await processDocument.StartProcessDocument(documentId, cancellationToken);

                documentIds.Add(documentId);
            }

            return Results.Ok(documentIds);

        }).DisableAntiforgery();

        documentEndPoints.MapGet("", async (
            [FromServices] IDocumentPredictionQueryService documentPredictionQueryService,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate) =>
        {
            return await documentPredictionQueryService.GetFilePredications(fromDate, toDate);
        });

        return endPoints;
    }
}

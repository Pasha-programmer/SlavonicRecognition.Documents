using Documents.Contract;
using Documents.Contract.Model;
using Documents.EndPoints.Models;
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
            [FromForm] AiModelType modelType,
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
                await using var memoryStream = new MemoryStream();
                await formFile.CopyToAsync(memoryStream, cancellationToken);
                var fileBlob = memoryStream.ToArray();

                var documentId = await documentCommandService.AddDocument(new()
                {
                    FileName = formFile.FileName,
                    FileBlob = fileBlob,
                }, cancellationToken);

                await processDocument.StartProcessDocument(documentId, fileBlob, modelType, cancellationToken);

                documentIds.Add(documentId);
            }

            return Results.Ok(documentIds);

        }).DisableAntiforgery();

        documentEndPoints.MapGet("", async (
            [FromServices] IDocumentPredictionQueryService documentPredictionQueryService,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate,
            [FromQuery] bool? hasProbability,
            CancellationToken cancellationToken) =>
        {
            var data = await documentPredictionQueryService.GetFilePredications(fromDate, toDate, hasProbability, cancellationToken);

            return Results.Ok(data);
        });

        documentEndPoints.MapPost("/reprocess", async (
            [FromBody] ReprocessParameters reprocessParameters,
            [FromServices] IProcessDocument processDocument,
            [FromServices] IDocumentQueryService documentQueryService,
            CancellationToken cancellationToken) =>
        {
            var document = await documentQueryService.GetDocument(reprocessParameters.DocumentId, cancellationToken);

            if (document == default)
            {
                return Results.BadRequest();
            }

            await processDocument.StartProcessDocument(
                reprocessParameters.DocumentId, 
                document.FileBlob, 
                reprocessParameters.ModelType, 
                cancellationToken);

            return Results.Ok(true);
        });

        return endPoints;
    }
}

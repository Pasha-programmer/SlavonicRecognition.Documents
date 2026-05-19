using Documents.Contract;
using Documents.Contract.Model;
using Documents.EndPoints.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.ComponentModel;
using System.Reflection;

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
            [FromForm] string modelType,
            [FromServices] IDocumentCommandService documentCommandService,
            [FromServices] IProcessDocument processDocument,
            CancellationToken cancellationToken) =>
        {
            if (images.Count == 0)
            {
                return Results.BadRequest("Файлы не обнаружены.");
            }

            // Конвертируем строку в enum
            var aiModelType = ConvertDescriptionToEnum(modelType);

            if (!aiModelType.HasValue)
            {
                return Results.BadRequest("Неверное наименование модели.");
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
                    SelectedModelType = aiModelType.Value,
                }, cancellationToken);

                await processDocument.StartProcessDocument(new()
                {
                    DocumentId = documentId,
                    Blob = fileBlob,
                    AiModelType = aiModelType.Value,
                }, cancellationToken);

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
            var documents = await documentQueryService.GetDocuments(reprocessParameters.DocumentIds, cancellationToken);

            if (documents.Count == 0)
            {
                return Results.BadRequest();
            }

            var foundDocumentIds = documents.Select(d => d.DocumentId).ToArray();
            var notFoundDocumentIds = reprocessParameters.DocumentIds.Except(foundDocumentIds).ToArray();

            var processingDocuments = documents.Select(d => new ProcessingDocument
            {
                DocumentId = d.DocumentId,
                Blob = d.FileBlob,
                AiModelType = reprocessParameters.ModelType
            }).ToArray();

            await processDocument.StartProcessDocuments(processingDocuments, cancellationToken);

            return Results.Ok(new
            {
                SendedDocumentsIds = foundDocumentIds,
                NotFoundDocumentsIds = notFoundDocumentIds,
            });
        });

        documentEndPoints.MapGet("/aiModelTypes", () =>
        {
            var aiModelTypes = Enum.GetValues<AiModelType>();

            return Results.Ok(aiModelTypes);
        });

        documentEndPoints.MapGet("/aiModelTypes/test-accuracy", async (
            [FromServices] IDocumentPredictionQueryService documentPredictionQueryService
            ) =>
        {
            var aiModelPredictions = await documentPredictionQueryService.GetAiModelTestAccuracy();

            return Results.Ok(aiModelPredictions.ToArray());
        });

        documentEndPoints.MapDelete("/{documentId}", async (
            [FromRoute] long documentId,
            [FromServices] IDocumentCommandService documentCommandService,
            CancellationToken cancellationToken
            ) =>
        {
            if (await documentCommandService.DeleteDocuments([documentId], cancellationToken))
            {
                return Results.Ok();
            }

            return Results.NotFound();
        });

        documentEndPoints.MapDelete("", async (
            [FromQuery] long[] documentIds,
            [FromServices] IDocumentQueryService documentQueryService,
            [FromServices] IDocumentCommandService documentCommandService,
            CancellationToken cancellationToken
            ) =>
        {
            if (await documentCommandService.DeleteDocuments(documentIds, cancellationToken))
            {
                return Results.Ok();
            }

            return Results.NotFound();
        });

        return endPoints;
    }

    private static AiModelType? ConvertDescriptionToEnum(string description)
    {
        foreach (var field in typeof(AiModelType).GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            var desc = field.GetCustomAttribute<DescriptionAttribute>()?.Description;
            if (desc == description)
            {
                return (AiModelType)field.GetValue(null)!;
            }
        }

        // Если не нашли по описанию, пробуем парсить напрямую
        if (Enum.TryParse<AiModelType>(description, out var result))
        {
            return result;
        }

        return null;
    }

}

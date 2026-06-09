using Documents.Contract.Document;
using Documents.Contract.DocumentPrediction;
using Documents.Contract.Model.DocumentPrediction;
using Documents.Contract.Model.Enums.AiModel;
using Documents.Contract.Model.ProcessDocument;
using Documents.Contract.ProcessDocument;
using Documents.EndPoints.Infrastructure;
using Documents.EndPoints.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Documents.EndPoints.Document;

internal static class DocumentEndPopints
{
    public static IEndpointRouteBuilder AddDocumentEndPopints(this IEndpointRouteBuilder apiEndPoint)
    {
        var documentEndPoints = apiEndPoint
            .MapGroup($"/documents");

        documentEndPoints.MapPost("/upload", async (
            [FromForm] IFormFileCollection images,
            [FromForm] string modelType,
            [FromForm] bool useTunedModels,
            [FromServices] IDocumentCommandService documentCommandService,
            [FromServices] IProcessDocumentService processDocument,
            CancellationToken cancellationToken) =>
        {
            if (images.Count == 0)
            {
                return Results.BadRequest("Файлы не обнаружены.");
            }

            // Конвертируем строку в enum
            var aiModelType = Tools.ConvertDescriptionToEnum(modelType);

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
                    UseTunedModels = useTunedModels,
                }, cancellationToken);

                documentIds.Add(documentId);
            }

            return Results.Ok(documentIds);

        }).DisableAntiforgery();

        documentEndPoints.MapGet("", async (
            [FromServices] IDocumentPredictionQueryService documentPredictionQueryService,
            [AsParameters] DocumentPredictionFilterParameters documentPredictionFilterParameters,
            CancellationToken cancellationToken) =>
        {
            var data = await documentPredictionQueryService.GetDocumentPredications(documentPredictionFilterParameters, cancellationToken);

            return Results.Ok(data);
        });

        documentEndPoints.MapPost("/reprocess", async (
            [FromBody] ReprocessParameters reprocessParameters,
            [FromServices] IProcessDocumentService processDocument,
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

            var processingDocuments = documents.Select(d => new DocumentToProcessDto
            {
                DocumentId = d.DocumentId,
                Blob = d.FileBlob,
                AiModelType = reprocessParameters.ModelType,
                UseTunedModels = reprocessParameters.UseTunedModels,
            }).ToArray();

            await processDocument.StartProcessDocuments(processingDocuments, cancellationToken);

            return Results.Ok(new
            {
                SendedDocumentsIds = foundDocumentIds,
                NotFoundDocumentsIds = notFoundDocumentIds,
            });
        });

        documentEndPoints.MapPost("/{id}/manual-prediction", async (
            [FromRoute] long id,
            [FromBody] ManualRecognition manualRecognition,
            [FromServices] IDocumentPredictionCommandService documentPredictionCommandService,
            CancellationToken cancellationToken) =>
        {
            var result = await documentPredictionCommandService.AddPredication([
                    new () {
                        DocumentId = id,
                        Label = manualRecognition.Label,
                        ModelType = null,
                        Probability = manualRecognition.Probability,
                        RecognitionType = RecognitionType.Manual,
                    }
                ], cancellationToken);

            if (result)
            {
                return Results.Ok(true);
            }

            return Results.Problem();
        });

        documentEndPoints.MapPut("/{id}/manual-prediction/{documentPredictionId}", async (
            [FromRoute] long id,
            [FromRoute] long documentPredictionId,
            [FromBody] ManualRecognition manualRecognition,
            [FromServices] IDocumentPredictionCommandService documentPredictionCommandService,
            CancellationToken cancellationToken) =>
        {
            var result = await documentPredictionCommandService.UpdatePredication([
                    new () {
                        Id = documentPredictionId,
                        DocumentId = id,
                        Label = manualRecognition.Label,
                        ModelType = null,
                        Probability = manualRecognition.Probability,
                        RecognitionType = RecognitionType.Manual,
                    }
                ], cancellationToken);

            if (result)
            {
                return Results.Ok(true);
            }

            return Results.Problem();
        });


        documentEndPoints.MapDelete("/{documentId}", async (
            [FromRoute] long documentId,
            [FromServices] IDocumentCommandService documentCommandService,
            CancellationToken cancellationToken
            ) =>
        {
            if (await documentCommandService.DeleteDocuments([documentId], cancellationToken))
            {
                return Results.Ok(true);
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
                return Results.Ok(true);
            }

            return Results.NotFound();
        });


        documentEndPoints.MapDelete("/{documentId}/document-prediction{documentPredictionId}", async (
            [FromRoute] long documentId,
            [FromRoute] long documentPredictionId,
            [FromServices] IDocumentPredictionCommandService documentPredictionCommandService,
            CancellationToken cancellationToken
            ) =>
        {
            if (documentId < 0)
            {
                return Results.NotFound();
            }

            if (await documentPredictionCommandService.DeletePredications([documentPredictionId], cancellationToken))
            {
                return Results.Ok(true);
            }

            return Results.NotFound();
        });

        return documentEndPoints;
    }
}

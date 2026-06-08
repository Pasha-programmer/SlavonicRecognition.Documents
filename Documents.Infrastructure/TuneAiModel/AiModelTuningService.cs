using Documents.Contract.DocumentPrediction;
using Documents.Contract.Model.AiModelTuning;
using Documents.Contract.Model.DocumentPrediction;
using Documents.Contract.TuneAiModel;
using Documents.Contract.TunedDocumentPrediction;
using Documents.Infrastructure.Contract;
using Documents.Infrastructure.Model;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Documents.Infrastructure.TuneAiModel;

/// <summary>
/// Реализация котракта для дообучения моделей ИИ.
/// </summary>
internal class AiModelTuningService(
    ILogger<AiModelTuningService> logger,
    IRabbitMqService rabbitMqService,
    IDocumentPredictionQueryService documentPredictionQueryService,
    ITunedDocumentPredictionQueryService tunedDocumentPredictionQueryService
    ) : IAiModelTuningService
{
    private readonly ILogger<AiModelTuningService> _logger = logger;
    private readonly IRabbitMqService _rabbitMqService = rabbitMqService;
    private readonly IDocumentPredictionQueryService _documentPredictionQueryService = documentPredictionQueryService;
    private readonly ITunedDocumentPredictionQueryService _tunedDocumentPredictionQueryService = tunedDocumentPredictionQueryService;

    private const string TUNE_REQUEST_QUEUE_NAME = "TuneRequest.Queue";

    /// <inheritdoc/>
    public async Task<bool> StartTuneAiModel(AiModelToTuningDto aiModelToTuningDto, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Получены на доообучение предсказания с IDs: {DocumentPredictionIds}", aiModelToTuningDto.DocumentPredictionIds);

        var aiModelToTuningModels = await GetDocumentPredictions(aiModelToTuningDto, cancellationToken);

        if (aiModelToTuningModels.Count == 0)
        {
            return false;
        }

        foreach (var aiModelToTuningModel in aiModelToTuningModels)
        {
            await _rabbitMqService.SendMessage(TUNE_REQUEST_QUEUE_NAME, aiModelToTuningModel, cancellationToken);
        }

        return true;
    }

    /// <inheritdoc/>
    private async Task<IReadOnlyCollection<AiModelToTuningModel>> GetDocumentPredictions(AiModelToTuningDto aiModelToTuningDto, CancellationToken cancellationToken = default)
    {
        var documentPredictionFilterParameters = new DocumentPredictionFilterParameters
        {
            DocumentPredictionIds = aiModelToTuningDto.DocumentPredictionIds,
        };

        var documentPredictions = await _documentPredictionQueryService.GetDocumentPredications(documentPredictionFilterParameters, cancellationToken);

        var notFoundDocumentPredictionIds = aiModelToTuningDto.DocumentPredictionIds
            .Except(documentPredictions.SelectMany(dp => dp.RecognitionResults.Select(rr => rr.Id!.Value)))
            .ToArray();
        if (notFoundDocumentPredictionIds.Length > 0)
        {
            _logger.LogError("Не не обнаружены предсказания: {DocumentPredictionIds}", notFoundDocumentPredictionIds);
        }
        else
        {
            _logger.LogInformation("Найдены все предсказания");
        }

        var tunedPredictions = (await _tunedDocumentPredictionQueryService.GetTunedDocumentPredictions(
            documentPredictionFilterParameters.DocumentPredictionIds,
            [aiModelToTuningDto.AiModelType],
            cancellationToken));

        var aiModelToTuningModels = documentPredictions
            .SelectMany(dp => dp.RecognitionResults
                .Where(rr => !tunedPredictions.Any(tp => tp.DocumentPredictionId == rr.DocumentId))
                .Select(rr => new AiModelToTuningModel
                {
                    AiModelType = aiModelToTuningDto.AiModelType,
                    FileBlob = dp.FileBlob,
                    Label = rr.Label,
                }))
            .ToArray();

        return aiModelToTuningModels;
    }
}

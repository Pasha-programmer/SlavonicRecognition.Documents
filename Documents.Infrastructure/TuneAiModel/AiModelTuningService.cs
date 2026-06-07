using Documents.Contract.DocumentPrediction;
using Documents.Contract.Model.AiModelTuning;
using Documents.Contract.Model.DocumentPrediction;
using Documents.Contract.TuneAiModel;
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
    IDocumentPredictionQueryService documentPredictionQueryService
    ) : IAiModelTuningService
{
    private readonly ILogger<AiModelTuningService> _logger = logger;
    private readonly IRabbitMqService _rabbitMqService = rabbitMqService;
    private readonly IDocumentPredictionQueryService _documentPredictionQueryService = documentPredictionQueryService;

    private const string TUNE_REQUEST_QUEUE_NAME = "TuneRequest.Queue";

    /// <inheritdoc/>
    public async Task<bool> StartTuneAiModel(AiModelToTuningDto aiModelToTuningDto, CancellationToken cancellationToken = default)
    {
        return await StartTuneAiModels([aiModelToTuningDto], cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<bool> StartTuneAiModels(IReadOnlyCollection<AiModelToTuningDto> aiModelsToTuningDto, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Получены на доообучение предсказания с IDs: {DocumentPredictionIds}", aiModelsToTuningDto
                .SelectMany(x => x.DocumentPredictionIds)
                .Distinct()
                .ToArray());

        var aiModelToTuningModels = await GetDocumentPredictions(aiModelsToTuningDto);

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
    private async Task<IReadOnlyCollection<AiModelToTuningModel>> GetDocumentPredictions(IReadOnlyCollection<AiModelToTuningDto> aiModelsToTuningDto)
    {
        var documentPredictionFilterParameters = new DocumentPredictionFilterParameters
        {
            DocumentPredictionIds = aiModelsToTuningDto
                .SelectMany(x => x.DocumentPredictionIds)
                .Distinct()
                .ToArray(),
        };

        var documentPredictions = await _documentPredictionQueryService.GetDocumentPredications(documentPredictionFilterParameters);

        var usedDocumentPredictionIds = new List<long>();

        var aiModelToTuningModels = documentPredictions.SelectMany(dp => dp.RecognitionResults
            .SelectMany(rr =>
            {
                var usedAiModelToTuningDtos = aiModelsToTuningDto
                    .Where(mt => mt.DocumentPredictionIds.Contains(rr.Id!.Value));

                if (usedAiModelToTuningDtos.Any())
                    usedDocumentPredictionIds.Add(rr.Id!.Value);
                else
                    return Array.Empty<AiModelToTuningModel>();

                return usedAiModelToTuningDtos
                    .Select(mt => new AiModelToTuningModel
                    {
                        AiModelType = mt.AiModelType,
                        FileBlob = dp.FileBlob,
                        Label = rr.Label,
                    })
                    .ToArray();
            })).ToArray();

        var notFoundDocumentPredictionIds = documentPredictionFilterParameters.DocumentPredictionIds.Except(usedDocumentPredictionIds).ToArray();
        if (notFoundDocumentPredictionIds.Length > 0)
        {
            _logger.LogError("Не не обнаружены предсказания: {DocumentPredictionIds}", notFoundDocumentPredictionIds);
        }
        else
        {
            _logger.LogInformation("Найдены все предсказания");
        }

        return aiModelToTuningModels;
    }
}

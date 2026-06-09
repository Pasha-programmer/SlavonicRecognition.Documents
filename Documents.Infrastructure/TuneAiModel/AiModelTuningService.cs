using Documents.Contract.DocumentPrediction;
using Documents.Contract.Model.AiModelTuning;
using Documents.Contract.Model.DocumentPrediction;
using Documents.Contract.Model.Enums.AiModel;
using Documents.Contract.Model.TunedDocumentPrediction;
using Documents.Contract.TuneAiModel;
using Documents.Contract.TunedDocumentPrediction;
using Documents.Database.Entity;
using Documents.Infrastructure.Contract;
using Documents.Infrastructure.Model;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Documents.Infrastructure.TuneAiModel;

/// <summary>
/// Реализация котракта для дообучения моделей ИИ.
/// </summary>
internal class AiModelTuningService(
    ILogger<AiModelTuningService> logger,
    IRabbitMqService rabbitMqService,
    IDocumentPredictionQueryService documentPredictionQueryService,
    ITunedDocumentPredictionQueryService tunedDocumentPredictionQueryService,
    ITunedDocumentPredictionCommandService tunedDocumentPredictionCommandService
    ) : IAiModelTuningService
{
    private readonly ILogger<AiModelTuningService> _logger = logger;
    private readonly IRabbitMqService _rabbitMqService = rabbitMqService;
    private readonly IDocumentPredictionQueryService _documentPredictionQueryService = documentPredictionQueryService;
    private readonly ITunedDocumentPredictionQueryService _tunedDocumentPredictionQueryService = tunedDocumentPredictionQueryService;
    private readonly ITunedDocumentPredictionCommandService _tunedDocumentPredictionCommandService = tunedDocumentPredictionCommandService;

    private const string TUNE_REQUEST_QUEUE_NAME = "TuneRequest.Queue";

    private readonly string[] _characters = [
        "Ⰰ", "Ⰱ", "Ⰲ", "Ⰳ", "Ⰴ", "Ⰵ", "Ⰶ", "Ⰷ", "Ⰸ", "Ⰺ", "Ⰻ", "Ⰼ", "Ⰽ", "Ⰾ", "Ⰿ",
        "Ⱀ", "Ⱁ", "Ⱂ", "Ⱃ", "Ⱄ", "Ⱅ", "Ⱆ", "Ⱇ", "Ⱈ", "Ⱉ", "Ⱊ", "Ⱋ", "Ⱌ", "Ⱍ", "Ⱎ",
        "Ⱏ", "ⰟⰊ", "Ⱐ", "Ⱑ", "Ⱒ", "Ⱓ", "Ⱔ", "Ⱖ", "Ⱗ", "Ⱘ", "Ⱙ", "Ⱚ", "Ⱛ"
    ];

    /// <inheritdoc/>
    public async Task<bool> StartTuneAiModel(AiModelToTuningDto aiModelToTuningDto, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Получены на доообучение предсказания с IDs: {DocumentPredictionIds}", aiModelToTuningDto.DocumentPredictionIds);

        var aiModelToTuningModels = await GetDocumentPredictions(aiModelToTuningDto, cancellationToken);

        if (aiModelToTuningModels.Count == 0)
        {
            return false;
        }

        var (foldersDir, newDataFileName) = PrepareDataset(aiModelToTuningModels);

        await _rabbitMqService.SendMessage(TUNE_REQUEST_QUEUE_NAME, new
        {
            FoldersDir = foldersDir,
            NewDataFileName = newDataFileName,
            AiModelType = aiModelToTuningDto.AiModelType,
        }, cancellationToken);

        await _tunedDocumentPredictionCommandService.AddTunedPredication(aiModelToTuningModels.Select(mt => new TunedPrediction
        {
            DocumentPredictionId = mt.DocumentPredictionId,
            ModelType = aiModelToTuningDto.AiModelType,
        }).ToArray());

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
                    DocumentPredictionId = rr.Id!.Value,
                    AiModelType = aiModelToTuningDto.AiModelType,
                    FileBlob = dp.FileBlob,
                    Label = rr.Label,
                }))
            .ToArray();

        return aiModelToTuningModels;
    }

    private (string FoldersDir, string NewDataFileName) PrepareDataset(IReadOnlyCollection<AiModelToTuningModel> models)
    {
        var rootDir = Path.Combine(Path.GetTempPath(), "tune_model");
        if (!Directory.Exists(rootDir))
        {
            Directory.CreateDirectory(rootDir);
        }

        rootDir = Path.Combine(rootDir, Guid.NewGuid().ToString());
        if (!Directory.Exists(rootDir))
        {
            Directory.CreateDirectory(rootDir);
        }

        var foldersDir = Path.Combine(rootDir, "train");
        if (!Directory.Exists(foldersDir))
        {
            Directory.CreateDirectory(foldersDir);
        }

        // 1. Создаем базовые папки на основе массива символов
        foreach (var character in _characters)
        {
            var dirPath = Path.Combine(foldersDir, character);

            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
        }

        // 2. Распределяем файлы по папкам
        foreach (var model in models)
        {
            // Проверяем, существует ли целевая папка (Label)
            // Важно: Path.Combine автоматически склеит путь
            var targetDirectory = Path.Combine(foldersDir, model.Label);

            // Формируем имя файла. 
            // Т.к. расширение не указано, можно добавить .bin или использовать AiModelType
            var fileName = $"{model.Label}_{model.AiModelType}_{Guid.NewGuid()}.jpg";
            var filePath = Path.Combine(targetDirectory, fileName);

            using var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            fs.Write(model.FileBlob.Span); // Работает напрямую с памятью
        }

        // 3. Копируем разметку
        var mapDir = Path.Combine(rootDir, "Map.csv");
        var mapCsvPath = Path.Combine(Directory.GetParent(Assembly.GetExecutingAssembly().Location)!.FullName, "CSV/Map.csv");
        File.Copy(mapCsvPath, mapDir);

        return (foldersDir, mapDir);
    }
}

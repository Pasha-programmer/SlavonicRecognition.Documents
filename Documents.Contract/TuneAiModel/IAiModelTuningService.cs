using Documents.Contract.Model.AiModelTuning;

namespace Documents.Contract.TuneAiModel;

/// <summary>
/// Котракт для дообучения моделей ИИ.
/// </summary>
public interface IAiModelTuningService
{
    /// <summary>
    /// Запустить дообучение модели ИИ
    /// </summary>
    /// <param name="aiModelToTuningDto">Модель команды на дообучение</param>
    /// <param name="cancellationToken"></param>
    /// <returns>true - если дообучение началось, иначе - false.</returns>
    public Task<bool> StartTuneAiModel(AiModelToTuningDto aiModelToTuningDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Запустить дообучение моделей ИИ
    /// </summary>
    /// <param name="aiModelsToTuningDto">Модели команды на дообучение</param>
    /// <param name="cancellationToken"></param>
    /// <returns>true - если дообучение началось, иначе - false.</returns>
    public Task<bool> StartTuneAiModels(IReadOnlyCollection<AiModelToTuningDto> aiModelsToTuningDto, CancellationToken cancellationToken = default);
}

using System.ComponentModel;
using System.Reflection;

namespace Documents.EndPoints.Infrastructure;

internal static class Tools
{
    public static Contract.Model.Enums.AiModel.AiModelType? ConvertDescriptionToEnum(string description)
    {
        foreach (var field in typeof(Contract.Model.Enums.AiModel.AiModelType).GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            var desc = field.GetCustomAttribute<DescriptionAttribute>()?.Description;
            if (desc == description)
            {
                return (Contract.Model.Enums.AiModel.AiModelType)field.GetValue(null)!;
            }
        }

        // Если не нашли по описанию, пробуем парсить напрямую
        if (Enum.TryParse<Contract.Model.Enums.AiModel.AiModelType>(description, out var result))
        {
            return result;
        }

        return null;
    }
}

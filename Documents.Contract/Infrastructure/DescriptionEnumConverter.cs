using System.ComponentModel;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Documents.Contract.Infrastructure;

public class DescriptionEnumConverter<T> : JsonConverter<T> where T : Enum
{
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString()
            ?? throw new JsonException("Value cannot be null");

        foreach (var field in typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            var description = field.GetCustomAttribute<DescriptionAttribute>()?.Description;
            if (description == value)
            {
                return (T)field.GetValue(null)!;
            }

            // Также поддерживаем прямой парсинг имени значения
            if (field.Name == value)
            {
                return (T)field.GetValue(null)!;
            }
        }

        // Попробуем парсить как число на случай обратной совместимости
        if (int.TryParse(value, out int intValue) && Enum.IsDefined(typeof(T), intValue))
        {
            return (T)Enum.ToObject(typeof(T), intValue);
        }

        throw new JsonException($"Unable to convert \"{value}\" to enum {typeof(T).Name}");
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        var field = value.GetType().GetField(value.ToString());
        var description = field?.GetCustomAttribute<DescriptionAttribute>()?.Description;

        writer.WriteStringValue(description ?? value.ToString());
    }
}

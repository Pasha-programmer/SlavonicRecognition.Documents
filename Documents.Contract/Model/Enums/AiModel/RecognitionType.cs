using Documents.Contract.Infrastructure;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Documents.Contract.Model.Enums.AiModel;

[JsonConverter(typeof(DescriptionEnumConverter<RecognitionType>))]
public enum RecognitionType : int
{
    [Description("Manual")]
    Manual = 1,

    [Description("Auto")]
    Auto = 2,
}

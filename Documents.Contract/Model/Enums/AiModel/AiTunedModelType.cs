using Documents.Contract.Infrastructure;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Documents.Contract.Model.Enums.AiModel;

[JsonConverter(typeof(DescriptionEnumConverter<AiModelType>))]
public enum AiTunedModelType : int
{
    [Description("All")]
    GlagoliticModelFull = 0,

    [Description("v1.1")]
    GlagoliticModelFullV1_1 = 11,

    [Description("v2.0")]
    GlagoliticModelFullV2_0 = 20,

    [Description("v2.1")]
    GlagoliticModelFullV2_1 = 21,

    [Description("v2.2")]
    GlagoliticModelFullV2_2 = 22,

    [Description("v3.0")]
    GlagoliticModelFullV3_0 = 30,

    [Description("v4.0")]
    GlagoliticModelFullV4_0 = 40,
}

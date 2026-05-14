using Documents.Contract.Infrastructure;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Documents.Contract.Model;

[JsonConverter(typeof(DescriptionEnumConverter<AiModelType>))]
public enum AiModelType : int
{
    [Description("v1.1")]
    GlagoliticModelFullV1_1 = 1,

    [Description("v2.0")]
    GlagoliticModelFullV2_0 = 2,

    [Description("v2.1")]
    GlagoliticModelFullV2_1 = 3,
    
    [Description("v3.0")]
    GlagoliticModelFullV3_0 = 4,
}

using System.Text.Json.Serialization;
using Swashbuckle.AspNetCore.TestSupport;

namespace Swashbuckle.AspNetCore.SwaggerGen.Test.Fixtures;

internal class JsonRequiredAnnotatedType
{

#if NET
    [JsonRequired]
#endif
    public string StringWithJsonRequired { get; set; }

#if NET
    [JsonRequired]
#endif
    public IntEnum IntEnumWithRequired { get; set; }

#if NET
    [JsonRequired]
#endif
    public IntEnum? NullableIntEnumWithRequired { get; set; }

#if NET
    [JsonRequired]
#endif
    public int IntWithRequired { get; set; }

#if NET
    [JsonRequired]
#endif
    public int? NullableIntWithRequired { get; set; }
}

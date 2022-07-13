using System.Numerics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace N7.Drand.JsonConverters;

public class BigIntegerAsNumberConverter : JsonConverter<BigInteger>
{
    public override BigInteger Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var asString = Encoding.UTF8.GetString(reader.ValueSpan);
        return BigInteger.Parse(asString);
    }

    public override void Write(Utf8JsonWriter writer, BigInteger value, JsonSerializerOptions options)
    {
        writer.WriteRawValue(value.ToString());
    }
}

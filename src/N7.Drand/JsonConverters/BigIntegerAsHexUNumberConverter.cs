using System.Numerics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace N7.Drand.JsonConverters;

public class BigIntegerAsHexUNumberConverter : JsonConverter<BigInteger>
{
    public override BigInteger Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return Encoding.UTF8
            .GetString(reader.ValueSpan)
            .FromHexToUnsignedBigInteger();
    }

    public override void Write(Utf8JsonWriter writer, BigInteger value, JsonSerializerOptions options)
    {
        if (value < BigInteger.Zero)
        {
            throw new ArgumentOutOfRangeException(
                paramName: nameof(value),
                message: "BigInteger must be positive number");
        }

        writer.WriteStringValue(value.ToString("x"));
    }
}
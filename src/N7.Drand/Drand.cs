using Microsoft.Extensions.Options;
using N7.Drand.JsonConverters;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace N7.Drand;

public record ChainInfo(
    [property: JsonPropertyName("public_key")] 
    String PublicKey,

    [property: JsonPropertyName("period")] 
    UInt64 Period,

    [property: JsonPropertyName("genesis_time")] 
    UInt64 GenesisTime,

    [property: JsonPropertyName("hash")] 
    String Hash
);

public record Round(
    [property: JsonPropertyName("round")]
    [property: JsonConverter(typeof(BigIntegerAsNumberConverter))]
    BigInteger Id,

    [property: JsonPropertyName("randomness")]
    [property: JsonConverter(typeof(BigIntegerAsHexUNumberConverter))]
    BigInteger Randomness,

    [property: JsonPropertyName("signature")] 
    String Signature,

    [property: JsonPropertyName("previous_signature")] 
    String PreviousSignature
);

public record QueryRoundById   (BigInteger Id);
public record QueryRoundByTime (UInt64 Timestamp);

public class Drand
{
    private readonly DrandOptions       options;
    private readonly IHttpClientFactory httpFactory;

    public Drand(IOptions<DrandOptions> options, IHttpClientFactory httpFactory)
    {
        this.options     = options.Value;
        this.httpFactory = httpFactory;
    }

    public async Task<Round> QueryAsync(QueryRoundById query, CancellationToken cancellationToken = default)
    {
        var http = httpFactory.CreateClient();
            http.BaseAddress = options.Providers[0];

        using var response = await http.GetAsync($"public/{query.Id}", cancellationToken);
                  response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        var obj  = JsonSerializer.Deserialize<Round>(json);

        return obj!;
    }
}

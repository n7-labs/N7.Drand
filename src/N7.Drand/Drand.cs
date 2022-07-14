using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace N7.Drand;

public sealed class ChainInfo
{
    [JsonConstructor]
    public ChainInfo(
        UInt64 genesisTime,
        String hash,
        UInt64 period,
        String publicKey)
    {
        GenesisTime = genesisTime;
        Hash        = hash;
        Period      = period;
        PublicKey   = publicKey;
    }

    [JsonPropertyName("genesis_time")]
    public UInt64 GenesisTime { get; }

    [JsonPropertyName("hash")]
    public String Hash { get; }

    [JsonPropertyName("period")]
    public UInt64 Period { get; }

    [JsonPropertyName("public_key")]
    public String PublicKey { get; }
}
public sealed class Round
{
    [JsonConstructor]
    public Round(
        UInt64 Id,
        String PreviousSignature,
        String Randomness,
        String Signature)
    {
        this.Id                = Id;
        this.PreviousSignature = PreviousSignature;
        this.Randomness        = Randomness;
        this.Signature         = Signature;
    }

    [JsonPropertyName("round")]
    public UInt64 Id { get; }

    [JsonPropertyName("previous_signature")]
    public String PreviousSignature { get; }

    [JsonPropertyName("randomness")]
    public String Randomness { get; }

    [JsonPropertyName("signature")]
    public String Signature { get; }
}

public sealed class QueryChainInfo
{
}
public sealed class QueryRoundById
{
    public QueryRoundById(UInt64 Id)
    {
        this.Id = Id;
    }

    public UInt64 Id { get; }
}
public sealed class QueryRoundTime
{
    public QueryRoundTime(UInt64 Id)
    {
        this.Id = Id;
    }

    public UInt64 Id { get; }
}
public sealed class QueryRoundByTime
{
    public QueryRoundByTime(UInt64 Timestamp)
    {
        this.Timestamp = Timestamp;
    }

    public ulong Timestamp { get; }
}

public class Drand
{
    private readonly DrandOptions       options;
    private readonly IHttpClientFactory httpFactory;

    public Drand(IOptions<DrandOptions> options, IHttpClientFactory httpFactory)
    {
        this.options     = options.Value;
        this.httpFactory = httpFactory;
    }

    public Task<ChainInfo> QueryAsync(QueryChainInfo query, CancellationToken ct = default)
        => GetAsync<ChainInfo>("info", ct);

    public Task<Round> QueryAsync(QueryRoundById query, CancellationToken ct = default)
        => GetAsync<Round>($"public/{query.Id}", ct);

    public Task<Round> QueryAsync(QueryRoundByTime query, CancellationToken ct = default)
    {
        var round = RoundAt(query.Timestamp);
        var req   = new QueryRoundById(round);
        var task  = QueryAsync(req, ct);

        return task;
    }

    public Task<UInt64> QueryAsync(QueryRoundTime query, CancellationToken ct = default)
    {
        var time = RoundTime(query.Id);
        return Task.FromResult(time);
    }

    private async Task<TResponse> GetAsync<TResponse>(String uri, CancellationToken ct = default)
    {
        var providerIndex = Random.Shared.Next(options.Providers.Count);
        var providerUri   = new Uri(options.Providers[providerIndex]);

        var http             = httpFactory.CreateClient();
            http.BaseAddress = providerUri;

        using var httpResponseMessage = await http.GetAsync(uri, ct);
                  httpResponseMessage.EnsureSuccessStatusCode();

        var json     = await httpResponseMessage.Content.ReadAsStringAsync(ct);
        var response = JsonSerializer.Deserialize<TResponse>(json);

        return response;
    }

    private UInt64 RoundAt(UInt64 timestamp)
    {
        if (timestamp < options.GenesisTime)
        {
            return 1;
        }

        var timeDiff = timestamp - options.GenesisTime;
        var periods  = Decimal.Divide(timeDiff, options.Period);
        var fp       = Decimal.Ceiling(periods);

        return (UInt64) fp + 1;
    }

    private UInt64 RoundTime(UInt64 id)
    {
        var adjustedRound = Math.Max(id, 0);
        var timestamp     = options.GenesisTime + ((adjustedRound - 1) * options.Period);

        return timestamp;
    }
}

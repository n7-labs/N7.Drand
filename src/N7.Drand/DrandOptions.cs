namespace N7.Drand;

public sealed class DrandOptions
{
    public List<String> Providers   { get; set; } = new();
    public String       PublicKey   { get; set; } = default!;
    public UInt64       Period      { get; set; } = default;
    public UInt64       GenesisTime { get; set; } = default;
    public String       Hash        { get; set; } = default!;
}
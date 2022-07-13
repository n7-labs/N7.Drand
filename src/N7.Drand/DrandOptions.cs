namespace N7.Drand;

public class DrandOptions
{
    public List<Uri> Providers { get; set; } = new();
    public ChainInfo ChainInfo { get; set; } = new(
        PublicKey   : String.Empty,
        Period      : default,
        GenesisTime : default,
        Hash        : String.Empty);
}

using Microsoft.Extensions.DependencyInjection;

namespace N7.Drand.Extensions;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddDrand(
        this IServiceCollection   services,
             Action<DrandOptions> configureOptions)
    {
        services.Configure(configureOptions);

        services.AddHttpClient();
        services.AddSingleton<Drand>();

        return services;
    }

    public static IServiceCollection AddDrand(
        this IServiceCollection services,
             DrandOptions       userOptions)
    {
        services
            .AddOptions<DrandOptions>()
            .Configure(options =>
            {
                options.GenesisTime = userOptions.GenesisTime;
                options.Hash        = userOptions.Hash;
                options.Period      = userOptions.Period;
                options.Providers   = userOptions.Providers;
                options.PublicKey   = userOptions.PublicKey;
            });

        services.AddHttpClient();
        services.AddSingleton<Drand>();

        return services;
    }    
}

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using N7.Drand;
using N7.Drand.Extensions;

var builder = Host
    .CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        var drandOptions = context.Configuration
            .GetSection("Drand")
            .Get<DrandOptions>();

        services
            .AddDrand(drandOptions);
    });

var host = builder.Build();

var drand = host.Services.GetRequiredService<Drand>();
//var req   = new QueryRoundByTime(1657763910);
var req   = new QueryRoundTime(2077784);
var res   = await drand.QueryAsync(req, CancellationToken.None);

_ = Task.Run(async () =>
{
    var currentRoundQuery = new QueryRoundById(0);
    var currentRound      = await drand.QueryAsync(currentRoundQuery, CancellationToken.None);

    Console.WriteLine($"Round: #{currentRound.Id} {currentRound.Randomness}");

    var nextRoundId = currentRound.Id + 1;

    do
    {
        var roundTimeQuery = new QueryRoundTime(nextRoundId);
        var nextRoundTime  = await drand.QueryAsync(roundTimeQuery, CancellationToken.None);

        Console.WriteLine($"Next round time: {nextRoundTime}");
        Console.WriteLine($"----------------------------------------------------------------------");

        var roundQuery = new QueryRoundByTime(nextRoundTime);
        var round      = await drand.QueryAsync(roundQuery, CancellationToken.None);

        Console.WriteLine($"Round: #{round.Id} {round.Randomness}");

        nextRoundId = round.Id + 1;

    } while (true);

});

await host.RunAsync();

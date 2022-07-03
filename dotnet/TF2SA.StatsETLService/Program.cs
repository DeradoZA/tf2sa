using TF2SA.StatsETLService;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((builderContext, services) =>
    {
        services.AddStatsETLService(builderContext.Configuration);
    })
    .Build();

await host.RunAsync();

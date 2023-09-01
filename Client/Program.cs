using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Interfaces;
using Newtonsoft.Json;

var invariant = "System.Data.SqlClient";
var connectionString =
    @"Data Source=localhost,1433;Initial Catalog=orleans-vanilla;
    Integrated Security=False;Pooling=False;Max Pool Size=200;
    MultipleActiveResultSets=True;
    User Id=sa;Password=Admin#1234;";

var builder = Host.CreateDefaultBuilder(args)
    .UseOrleansClient(silo =>
    {
        silo.UseAdoNetClustering(options =>
        {
            options.Invariant = invariant;
            options.ConnectionString = connectionString;
        });
    })
    .ConfigureLogging(logging => logging.AddConsole())
    .UseConsoleLifetime();

using var host = builder.Build();
await host.StartAsync();

var client = host.Services.GetRequiredService<IClusterClient>();

var friend = client.GetGrain<IHello>(Guid.Parse("1ddc41accadc4fe6a9b72edfd88fb9b7"));

string response = await friend.SayHello("Hello!");

//var response = await friend.GetGreetings();

await host.StopAsync();

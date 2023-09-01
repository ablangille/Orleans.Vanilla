using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var invariant = "System.Data.SqlClient";
var connectionString =
    @"Data Source=localhost,1433;Initial Catalog=orleans-vanilla;
    Integrated Security=False;Pooling=False;Max Pool Size=200;
    MultipleActiveResultSets=True;
    User Id=sa;Password=Admin#1234;";

var builder = Host.CreateDefaultBuilder(args)
    .UseOrleans(silo =>
    {
        silo.UseAdoNetClustering(options =>
            {
                options.Invariant = invariant;
                options.ConnectionString = connectionString;
            })
            .UseAdoNetReminderService(options =>
            {
                options.Invariant = invariant;
                options.ConnectionString = connectionString;
            })
            .AddAdoNetGrainStorage(
                "GrainStorageForTest",
                options =>
                {
                    options.Invariant = invariant;
                    options.ConnectionString = connectionString;
                }
            );
    })
    .ConfigureLogging(logging => logging.AddConsole())
    .UseConsoleLifetime();

using var host = builder.Build();

await host.RunAsync();

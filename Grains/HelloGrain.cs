using Interfaces;
using Microsoft.Extensions.Logging;
using Orleans.Providers;

namespace Grains;

[StorageProvider(ProviderName = "GrainStorageForTest")]
public class HelloGrain : Grain<HelloState>, IHello
{
    private readonly ILogger _logger;

    public HelloGrain(ILogger<HelloGrain> logger) => _logger = logger;

    /*public override async Task<Task> OnActivateAsync(CancellationToken cancellationToken)
    {
        var t = RegisterTimer(
            state =>
            {
                Console.WriteLine($"this is activated on {DateTime.Now.ToString()}");
                return Task.CompletedTask;
            },
            null!,
            TimeSpan.FromSeconds(10),
            TimeSpan.FromSeconds(10)
        );

        await Task.FromResult(t);

        return base.OnActivateAsync(cancellationToken);
    }*/

    public async ValueTask<string> SayHello(string greeting)
    {
        _logger.LogInformation($"SayHello received greeting: {greeting}");

        _logger.LogInformation($"On Grain ContainerId {State.ContainerId}");

        State.Greetings.Add(greeting);

        State.ContainerId = State.ContainerId == Guid.Empty ? Guid.NewGuid() : State.ContainerId;

        await base.WriteStateAsync();

        var grainId = this.GetGrainId().ToString();

        return await ValueTask.FromResult(
            $"Client said: {greeting}, so HelloGrain says: my id is {grainId}!"
        );
    }

    public async ValueTask<IList<string>> GetGreetings()
    {
        var grainId = this.GetGrainId().ToString();
        
        _logger.LogInformation($"GetGreetings received request for grain {grainId}");

        return await ValueTask.FromResult(State.Greetings);
    }
}

public class HelloState
{
    public HelloState()
    {
        Greetings = new List<string>();
    }

    public Guid ContainerId { get; set; }
    public List<string> Greetings { get; set; }
}

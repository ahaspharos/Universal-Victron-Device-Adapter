using VictronBridge.Core.Events;

namespace VictronBridge.Core.Abstractions;

public interface IDataSource
{
    string Name { get; }

    event EventHandler<SourceValueReceivedEventArgs>? ValueReceived;

    Task StartAsync(CancellationToken cancellationToken);

    Task StopAsync(CancellationToken cancellationToken);
}

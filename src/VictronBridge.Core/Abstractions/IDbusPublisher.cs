namespace VictronBridge.Core.Abstractions;

public interface IDbusPublisher
{
    Task PublishAsync(
        string serviceName,
        IReadOnlyDictionary<string, object?> values,
        CancellationToken cancellationToken = default);
}

namespace VictronBridge.Core.Pipeline;

public interface IPipelineStep
{
    Task ExecuteAsync(
        PipelineContext context,
        Func<PipelineContext, Task> next,
        CancellationToken cancellationToken);
}

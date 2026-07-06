using Microsoft.Extensions.Options;

namespace VictronBridge.Configuration;

public sealed class BridgeOptionsValidator : IValidateOptions<BridgeOptions>
{
    public ValidateOptionsResult Validate(string? name, BridgeOptions options)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(options.Source.Type))
            errors.Add("Source.Type is required.");

        if (string.IsNullOrWhiteSpace(options.Device.Type))
            errors.Add("Device.Type is required.");

        if (string.IsNullOrWhiteSpace(options.Device.ServiceName))
            errors.Add("Device.ServiceName is required.");

        return errors.Count > 0
            ? ValidateOptionsResult.Fail(errors)
            : ValidateOptionsResult.Success;
    }
}

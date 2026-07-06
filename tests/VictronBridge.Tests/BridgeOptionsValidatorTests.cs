using Microsoft.Extensions.Options;
using VictronBridge.Configuration;

namespace VictronBridge.Tests;

public sealed class BridgeOptionsValidatorTests
{
    private readonly BridgeOptionsValidator _validator = new();

    [Fact]
    public void Validate_ValidOptions_ReturnsSuccess()
    {
        var options = new BridgeOptions
        {
            Source = new SourceOptions { Type = "mqtt", Host = "localhost" },
            Device = new DeviceOptions
            {
                Type = "battery",
                ServiceName = "com.victronenergy.battery.test"
            }
        };

        var result = _validator.Validate(null, options);

        Assert.Equal(ValidateOptionsResult.Success, result);
    }

    [Fact]
    public void Validate_MissingSourceType_ReturnsFail()
    {
        var options = new BridgeOptions
        {
            Source = new SourceOptions { Host = "localhost" },
            Device = new DeviceOptions
            {
                Type = "battery",
                ServiceName = "com.victronenergy.battery.test"
            }
        };

        var result = _validator.Validate(null, options);

        Assert.True(result.Failed);
        Assert.Contains(result.Failures!, f => f.Contains("Source.Type"));
    }

    [Fact]
    public void Validate_MissingDeviceType_ReturnsFail()
    {
        var options = new BridgeOptions
        {
            Source = new SourceOptions { Type = "mqtt", Host = "localhost" },
            Device = new DeviceOptions { ServiceName = "com.victronenergy.battery.test" }
        };

        var result = _validator.Validate(null, options);

        Assert.True(result.Failed);
        Assert.Contains(result.Failures!, f => f.Contains("Device.Type"));
    }

    [Fact]
    public void Validate_MissingServiceName_ReturnsFail()
    {
        var options = new BridgeOptions
        {
            Source = new SourceOptions { Type = "mqtt", Host = "localhost" },
            Device = new DeviceOptions { Type = "battery" }
        };

        var result = _validator.Validate(null, options);

        Assert.True(result.Failed);
        Assert.Contains(result.Failures!, f => f.Contains("Device.ServiceName"));
    }

    [Fact]
    public void Validate_MultipleErrors_ReportsAll()
    {
        var options = new BridgeOptions();

        var result = _validator.Validate(null, options);

        Assert.True(result.Failed);
        Assert.Equal(3, result.Failures!.Count());
    }
}

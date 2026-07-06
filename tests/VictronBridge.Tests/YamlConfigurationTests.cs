using Microsoft.Extensions.Configuration;
using VictronBridge.Configuration;
using VictronBridge.Configuration.Yaml;

namespace VictronBridge.Tests;

public sealed class YamlConfigurationTests
{
    [Fact]
    public void LoadYaml_ValidBridgeConfig_BindsToBridgeOptions()
    {
        var yaml = """
            Bridge:
              Source:
                Type: mqtt
                Host: 192.168.1.100
                Port: 1883
              Device:
                Type: battery
                ServiceName: com.victronenergy.battery.test
                ProductName: Test Battery
                DeviceInstance: 1
              Mappings:
                soc: battery/soc
                voltage: battery/voltage
            """;

        var config = BuildConfigFromYaml(yaml);
        var options = config.GetSection(BridgeOptions.SectionName).Get<BridgeOptions>();

        Assert.NotNull(options);
        Assert.Equal("mqtt", options.Source.Type);
        Assert.Equal("192.168.1.100", options.Source.Host);
        Assert.Equal(1883, options.Source.Port);
        Assert.Equal("battery", options.Device.Type);
        Assert.Equal("com.victronenergy.battery.test", options.Device.ServiceName);
        Assert.Equal("Test Battery", options.Device.ProductName);
        Assert.Equal(1, options.Device.DeviceInstance);
        Assert.Equal("battery/soc", options.Mappings["soc"]);
        Assert.Equal("battery/voltage", options.Mappings["voltage"]);
    }

    [Fact]
    public void LoadYaml_MissingOptionalFields_UsesDefaults()
    {
        var yaml = """
            Bridge:
              Source:
                Type: mqtt
                Host: 192.168.1.1
              Device:
                Type: battery
                ServiceName: com.victronenergy.battery.default
            """;

        var config = BuildConfigFromYaml(yaml);
        var options = config.GetSection(BridgeOptions.SectionName).Get<BridgeOptions>();

        Assert.NotNull(options);
        Assert.Equal(0, options.Source.Port);
        Assert.Null(options.Source.Username);
        Assert.Null(options.Source.Password);
        Assert.False(options.Source.UseTls);
        Assert.Equal("VictronBridge Device", options.Device.ProductName);
        Assert.Equal(0, options.Device.DeviceInstance);
        Assert.Empty(options.Mappings);
    }

    [Fact]
    public void LoadYaml_MqttSourceOptions_BindsCorrectly()
    {
        var yaml = """
            Source:
              Mqtt:
                Host: broker.local
                Port: 8883
                ClientId: bridge01
                KeepAliveSeconds: 30
                UseTls: true
                Username: user
                Password: pass
                Topics:
                  - battery/#
                  - solar/#
            """;

        var config = BuildConfigFromYaml(yaml);
        var options = config.GetSection(VictronBridge.Configuration.Sources.MqttSourceOptions.SectionName)
            .Get<VictronBridge.Configuration.Sources.MqttSourceOptions>();

        Assert.NotNull(options);
        Assert.Equal("broker.local", options.Host);
        Assert.Equal(8883, options.Port);
        Assert.Equal("bridge01", options.ClientId);
        Assert.Equal(30, options.KeepAliveSeconds);
        Assert.True(options.UseTls);
        Assert.Equal("user", options.Username);
        Assert.Equal("pass", options.Password);
        Assert.Equal(2, options.Topics.Count);
        Assert.Contains("battery/#", options.Topics);
        Assert.Contains("solar/#", options.Topics);
    }

    [Fact]
    public void LoadYaml_ModbusTcpSourceOptions_BindsCorrectly()
    {
        var yaml = """
            Source:
              ModbusTcp:
                Host: 10.0.0.5
                Port: 502
                UnitId: 2
                PollingIntervalSeconds: 10
            """;

        var config = BuildConfigFromYaml(yaml);
        var options = config.GetSection(VictronBridge.Configuration.Sources.ModbusTcpSourceOptions.SectionName)
            .Get<VictronBridge.Configuration.Sources.ModbusTcpSourceOptions>();

        Assert.NotNull(options);
        Assert.Equal("10.0.0.5", options.Host);
        Assert.Equal(502, options.Port);
        Assert.Equal(2, options.UnitId);
        Assert.Equal(10, options.PollingIntervalSeconds);
    }

    [Fact]
    public void LoadYaml_ModbusRtuSourceOptions_BindsCorrectly()
    {
        var yaml = """
            Source:
              ModbusRtu:
                SerialPort: /dev/ttyUSB0
                BaudRate: 9600
                UnitId: 1
                PollingIntervalSeconds: 5
            """;

        var config = BuildConfigFromYaml(yaml);
        var options = config.GetSection(VictronBridge.Configuration.Sources.ModbusRtuSourceOptions.SectionName)
            .Get<VictronBridge.Configuration.Sources.ModbusRtuSourceOptions>();

        Assert.NotNull(options);
        Assert.Equal("/dev/ttyUSB0", options.SerialPort);
        Assert.Equal(9600, options.BaudRate);
        Assert.Equal(1, options.UnitId);
        Assert.Equal(5, options.PollingIntervalSeconds);
    }

    [Fact]
    public void LoadYaml_HttpSourceOptions_BindsCorrectly()
    {
        var yaml = """
            Source:
              Http:
                BaseUrl: http://device.local/api
                PollingIntervalSeconds: 15
                Headers:
                  Authorization: Bearer token123
                  Accept: application/json
            """;

        var config = BuildConfigFromYaml(yaml);
        var options = config.GetSection(VictronBridge.Configuration.Sources.HttpSourceOptions.SectionName)
            .Get<VictronBridge.Configuration.Sources.HttpSourceOptions>();

        Assert.NotNull(options);
        Assert.Equal("http://device.local/api", options.BaseUrl);
        Assert.Equal(15, options.PollingIntervalSeconds);
        Assert.Equal("Bearer token123", options.Headers["Authorization"]);
        Assert.Equal("application/json", options.Headers["Accept"]);
    }

    [Fact]
    public void LoadYaml_EmptyDocument_ReturnsEmptyConfig()
    {
        var config = BuildConfigFromYaml(string.Empty);
        var options = config.GetSection(BridgeOptions.SectionName).Get<BridgeOptions>();

        Assert.Null(options);
    }

    private static IConfiguration BuildConfigFromYaml(string yaml)
    {
        var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(yaml));
        return new ConfigurationBuilder()
            .AddYamlStream(stream)
            .Build();
    }
}

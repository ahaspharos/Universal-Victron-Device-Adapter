using VictronBridge.Configuration;
using VictronBridge.Configuration.Yaml;
using VictronBridge.DBus;
using VictronBridge.Devices.Battery;
using VictronBridge.Host;
using VictronBridge.Mapping;
using VictronBridge.Sources.Mqtt;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration
    .AddYamlFile("bridge.yaml", optional: true, reloadOnChange: true)
    .AddYamlFile("bridge.override.yaml", optional: true, reloadOnChange: true);

builder.Services.AddBridgeConfiguration(builder.Configuration);
builder.Services.AddMqttSource();
builder.Services.AddBatteryDevice();
builder.Services.AddVenusDbusPublisher();
builder.Services.AddMappingEngine();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();


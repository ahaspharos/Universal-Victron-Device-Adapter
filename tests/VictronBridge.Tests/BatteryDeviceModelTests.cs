using Microsoft.Extensions.Options;
using VictronBridge.Configuration;
using VictronBridge.Devices.Battery;

namespace VictronBridge.Tests;

public sealed class BatteryDeviceModelTests
{
    private static BatteryDeviceModel CreateModel(
        string serviceName = "com.victronenergy.battery.test",
        string productName = "Test Battery",
        int deviceInstance = 0)
    {
        var options = Options.Create(new DeviceOptions
        {
            Type = "battery",
            ServiceName = serviceName,
            ProductName = productName,
            DeviceInstance = deviceInstance
        });
        return new BatteryDeviceModel(options);
    }

    // ── Identity ─────────────────────────────────────────────────────────

    [Fact]
    public void ServiceName_ReturnsValueFromOptions()
    {
        var model = CreateModel(serviceName: "com.victronenergy.battery.mqtt01");
        Assert.Equal("com.victronenergy.battery.mqtt01", model.ServiceName);
    }

    [Fact]
    public void DeviceType_ReturnsVictronBatteryPrefix()
    {
        var model = CreateModel();
        Assert.Equal("com.victronenergy.battery", model.DeviceType);
    }

    // ── Seed ─────────────────────────────────────────────────────────────

    [Fact]
    public void Constructor_SeedsProductNameFromOptions()
    {
        var model = CreateModel(productName: "MQTT Battery");
        Assert.Equal("MQTT Battery", model.ProductName);
    }

    [Fact]
    public void Constructor_SeedsDeviceInstanceFromOptions()
    {
        var model = CreateModel(deviceInstance: 2);
        Assert.Equal(2, model.DeviceInstance);
    }

    [Fact]
    public void Constructor_SeedsConnectedAsZero()
    {
        var model = CreateModel();
        Assert.Equal(0, model.Connected);
    }

    // ── D-Bus paths ───────────────────────────────────────────────────────

    [Fact]
    public void DbusValues_ContainsSeededPaths()
    {
        var model = CreateModel(productName: "Battery", deviceInstance: 1);
        var values = model.GetDbusValues();

        Assert.True(values.ContainsKey("/ProductName"));
        Assert.True(values.ContainsKey("/DeviceInstance"));
        Assert.True(values.ContainsKey("/Connected"));
    }

    [Fact]
    public void DbusPath_SocMapsToCorrectPath()
    {
        var model = CreateModel();
        model.Soc = 80.0;
        Assert.Equal(80.0, model.GetDbusValues()[BatteryDeviceModel.DbusPath.Soc]);
    }

    [Fact]
    public void DbusPath_VoltageMapsToCorrectPath()
    {
        var model = CreateModel();
        model.Voltage = 52.4;
        Assert.Equal(52.4, model.GetDbusValues()[BatteryDeviceModel.DbusPath.Voltage]);
    }

    [Fact]
    public void DbusPath_CurrentMapsToCorrectPath()
    {
        var model = CreateModel();
        model.Current = -14.2;
        Assert.Equal(-14.2, model.GetDbusValues()[BatteryDeviceModel.DbusPath.Current]);
    }

    [Fact]
    public void DbusPath_PowerMapsToCorrectPath()
    {
        var model = CreateModel();
        model.Power = -750.0;
        Assert.Equal(-750.0, model.GetDbusValues()[BatteryDeviceModel.DbusPath.Power]);
    }

    [Fact]
    public void DbusPath_TemperatureMapsToCorrectPath()
    {
        var model = CreateModel();
        model.Temperature = 25.5;
        Assert.Equal(25.5, model.GetDbusValues()[BatteryDeviceModel.DbusPath.Temperature]);
    }

    [Theory]
    [InlineData("/Soc")]
    [InlineData("/Dc/0/Voltage")]
    [InlineData("/Dc/0/Current")]
    [InlineData("/Dc/0/Power")]
    [InlineData("/Dc/0/Temperature")]
    [InlineData("/Capacity")]
    [InlineData("/InstalledCapacity")]
    [InlineData("/TimeToGo")]
    [InlineData("/Connected")]
    [InlineData("/State")]
    [InlineData("/ProductName")]
    [InlineData("/DeviceInstance")]
    [InlineData("/Alarms/LowVoltage")]
    [InlineData("/Alarms/HighVoltage")]
    [InlineData("/Alarms/LowSoc")]
    [InlineData("/Alarms/LowTemperature")]
    [InlineData("/Alarms/HighTemperature")]
    [InlineData("/History/MinimumCellVoltage")]
    [InlineData("/History/MaximumCellVoltage")]
    [InlineData("/History/ChargedEnergy")]
    [InlineData("/History/DischargedEnergy")]
    public void DbusPath_ConstantsMatchExpectedVictronPaths(string expectedPath)
    {
        var paths = typeof(BatteryDeviceModel.DbusPath)
            .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
            .Select(f => f.GetValue(null) as string)
            .ToList();

        Assert.Contains(expectedPath, paths);
    }

    // ── ApplyValues ───────────────────────────────────────────────────────

    [Fact]
    public void ApplyValues_SetsAllCoreProperties()
    {
        var model = CreateModel();
        model.ApplyValues(new Dictionary<string, object?>
        {
            ["soc"] = 78.0,
            ["voltage"] = 52.8,
            ["current"] = -14.2,
            ["power"] = -750.0,
            ["temperature"] = 25.0,
        });

        Assert.Equal(78.0, model.Soc);
        Assert.Equal(52.8, model.Voltage);
        Assert.Equal(-14.2, model.Current);
        Assert.Equal(-750.0, model.Power);
        Assert.Equal(25.0, model.Temperature);
    }

    [Fact]
    public void ApplyValues_SetsCapacityAndTimeToGo()
    {
        var model = CreateModel();
        model.ApplyValues(new Dictionary<string, object?>
        {
            ["capacity"] = 100.0,
            ["installedcapacity"] = 200.0,
            ["timetogo"] = 7200,
        });

        Assert.Equal(100.0, model.Capacity);
        Assert.Equal(200.0, model.InstalledCapacity);
        Assert.Equal(7200, model.TimeToGo);
    }

    [Fact]
    public void ApplyValues_SetsStateAndConnected()
    {
        var model = CreateModel();
        model.ApplyValues(new Dictionary<string, object?>
        {
            ["connected"] = 1,
            ["state"] = 2,
        });

        Assert.Equal(1, model.Connected);
        Assert.Equal(2, model.State);
    }

    [Fact]
    public void ApplyValues_SetsAlarmPaths()
    {
        var model = CreateModel();
        model.ApplyValues(new Dictionary<string, object?>
        {
            ["alarmlowvoltage"] = 1,
            ["alarmhighvoltage"] = 0,
            ["alarmlowsoc"] = 1,
            ["alarmlowtemperature"] = 0,
            ["alarmhightemperature"] = 0,
        });

        Assert.Equal(1, model.AlarmLowVoltage);
        Assert.Equal(0, model.AlarmHighVoltage);
        Assert.Equal(1, model.AlarmLowSoc);
        Assert.Equal(0, model.AlarmLowTemperature);
        Assert.Equal(0, model.AlarmHighTemperature);
    }

    [Fact]
    public void ApplyValues_SetsHistoryPaths()
    {
        var model = CreateModel();
        model.ApplyValues(new Dictionary<string, object?>
        {
            ["minimumcellvoltage"] = 3.1,
            ["maximumcellvoltage"] = 4.2,
            ["chargedenergy"] = 500.0,
            ["dischargedenergy"] = 480.0,
        });

        Assert.Equal(3.1, model.MinimumCellVoltage);
        Assert.Equal(4.2, model.MaximumCellVoltage);
        Assert.Equal(500.0, model.ChargedEnergy);
        Assert.Equal(480.0, model.DischargedEnergy);
    }

    [Fact]
    public void ApplyValues_KeyCaseInsensitive()
    {
        var model = CreateModel();
        model.ApplyValues(new Dictionary<string, object?>
        {
            ["SOC"] = 55.0,
            ["VOLTAGE"] = 51.0,
        });

        Assert.Equal(55.0, model.Soc);
        Assert.Equal(51.0, model.Voltage);
    }

    [Fact]
    public void ApplyValues_NullValue_SetsPropertyToNull()
    {
        var model = CreateModel();
        model.Soc = 80.0;
        model.ApplyValues(new Dictionary<string, object?> { ["soc"] = null });

        Assert.Null(model.Soc);
    }

    [Fact]
    public void ApplyValues_LongValue_ConvertsToDouble()
    {
        var model = CreateModel();
        model.ApplyValues(new Dictionary<string, object?> { ["soc"] = 82L });

        Assert.Equal(82.0, model.Soc);
    }

    [Fact]
    public void ApplyValues_UnknownKey_IsIgnored()
    {
        var model = CreateModel();
        var ex = Record.Exception(() =>
            model.ApplyValues(new Dictionary<string, object?> { ["unknownfield"] = 42 }));

        Assert.Null(ex);
    }

    // ── GetDbusValues ─────────────────────────────────────────────────────

    [Fact]
    public void GetDbusValues_ReflectsSetProperties()
    {
        var model = CreateModel();
        model.Soc = 90.0;
        model.Voltage = 53.6;

        var values = model.GetDbusValues();
        Assert.Equal(90.0, values["/Soc"]);
        Assert.Equal(53.6, values["/Dc/0/Voltage"]);
    }

    [Fact]
    public void GetDbusValues_ProductName_ContainsValueFromOptions()
    {
        var model = CreateModel(productName: "My Battery");
        Assert.Equal("My Battery", model.GetDbusValues()["/ProductName"]);
    }
}

using Microsoft.Extensions.Options;
using VictronBridge.Configuration;
using VictronBridge.Models;

namespace VictronBridge.Devices.Battery;

public sealed class BatteryDeviceModel : DeviceModelBase
{
    private readonly DeviceOptions _options;

    public BatteryDeviceModel(IOptions<DeviceOptions> options)
    {
        _options = options.Value;
        Seed();
    }

    public override string ServiceName => _options.ServiceName;
    public override string DeviceType => "com.victronenergy.battery";

    // ── DC measurements ──────────────────────────────────────────────────

    public double? Soc
    {
        get => GetValue(DbusPath.Soc) as double?;
        set => SetValue(DbusPath.Soc, value);
    }

    public double? Voltage
    {
        get => GetValue(DbusPath.Voltage) as double?;
        set => SetValue(DbusPath.Voltage, value);
    }

    public double? Current
    {
        get => GetValue(DbusPath.Current) as double?;
        set => SetValue(DbusPath.Current, value);
    }

    public double? Power
    {
        get => GetValue(DbusPath.Power) as double?;
        set => SetValue(DbusPath.Power, value);
    }

    public double? Temperature
    {
        get => GetValue(DbusPath.Temperature) as double?;
        set => SetValue(DbusPath.Temperature, value);
    }

    // ── Capacity ─────────────────────────────────────────────────────────

    public double? Capacity
    {
        get => GetValue(DbusPath.Capacity) as double?;
        set => SetValue(DbusPath.Capacity, value);
    }

    public double? InstalledCapacity
    {
        get => GetValue(DbusPath.InstalledCapacity) as double?;
        set => SetValue(DbusPath.InstalledCapacity, value);
    }

    public int? TimeToGo
    {
        get => GetValue(DbusPath.TimeToGo) as int?;
        set => SetValue(DbusPath.TimeToGo, value);
    }

    // ── State ────────────────────────────────────────────────────────────

    public int? Connected
    {
        get => GetValue(DbusPath.Connected) as int?;
        set => SetValue(DbusPath.Connected, value);
    }

    public int? State
    {
        get => GetValue(DbusPath.State) as int?;
        set => SetValue(DbusPath.State, value);
    }

    // ── Device info ──────────────────────────────────────────────────────

    public string? ProductName
    {
        get => GetValue(DbusPath.ProductName) as string;
        set => SetValue(DbusPath.ProductName, value);
    }

    public int? DeviceInstance
    {
        get => GetValue(DbusPath.DeviceInstance) as int?;
        set => SetValue(DbusPath.DeviceInstance, value);
    }

    // ── Alarms ───────────────────────────────────────────────────────────

    public int? AlarmLowVoltage
    {
        get => GetValue(DbusPath.AlarmLowVoltage) as int?;
        set => SetValue(DbusPath.AlarmLowVoltage, value);
    }

    public int? AlarmHighVoltage
    {
        get => GetValue(DbusPath.AlarmHighVoltage) as int?;
        set => SetValue(DbusPath.AlarmHighVoltage, value);
    }

    public int? AlarmLowSoc
    {
        get => GetValue(DbusPath.AlarmLowSoc) as int?;
        set => SetValue(DbusPath.AlarmLowSoc, value);
    }

    public int? AlarmLowTemperature
    {
        get => GetValue(DbusPath.AlarmLowTemperature) as int?;
        set => SetValue(DbusPath.AlarmLowTemperature, value);
    }

    public int? AlarmHighTemperature
    {
        get => GetValue(DbusPath.AlarmHighTemperature) as int?;
        set => SetValue(DbusPath.AlarmHighTemperature, value);
    }

    // ── History ──────────────────────────────────────────────────────────

    public double? MinimumCellVoltage
    {
        get => GetValue(DbusPath.MinimumCellVoltage) as double?;
        set => SetValue(DbusPath.MinimumCellVoltage, value);
    }

    public double? MaximumCellVoltage
    {
        get => GetValue(DbusPath.MaximumCellVoltage) as double?;
        set => SetValue(DbusPath.MaximumCellVoltage, value);
    }

    public double? ChargedEnergy
    {
        get => GetValue(DbusPath.ChargedEnergy) as double?;
        set => SetValue(DbusPath.ChargedEnergy, value);
    }

    public double? DischargedEnergy
    {
        get => GetValue(DbusPath.DischargedEnergy) as double?;
        set => SetValue(DbusPath.DischargedEnergy, value);
    }

    public override void ApplyValues(IReadOnlyDictionary<string, object?> values)
    {
        foreach (var (key, value) in values)
        {
            switch (key.ToLowerInvariant())
            {
                case "soc":
                    Soc = ToDouble(value);
                    break;
                case "voltage":
                    Voltage = ToDouble(value);
                    break;
                case "current":
                    Current = ToDouble(value);
                    break;
                case "power":
                    Power = ToDouble(value);
                    break;
                case "temperature":
                    Temperature = ToDouble(value);
                    break;
                case "capacity":
                    Capacity = ToDouble(value);
                    break;
                case "installedcapacity":
                    InstalledCapacity = ToDouble(value);
                    break;
                case "timetogo":
                    TimeToGo = ToInt(value);
                    break;
                case "connected":
                    Connected = ToInt(value);
                    break;
                case "state":
                    State = ToInt(value);
                    break;
                case "productname":
                    ProductName = value?.ToString();
                    break;
                case "alarmlowvoltage":
                    AlarmLowVoltage = ToInt(value);
                    break;
                case "alarmhighvoltage":
                    AlarmHighVoltage = ToInt(value);
                    break;
                case "alarmlowsoc":
                    AlarmLowSoc = ToInt(value);
                    break;
                case "alarmlowtemperature":
                    AlarmLowTemperature = ToInt(value);
                    break;
                case "alarmhightemperature":
                    AlarmHighTemperature = ToInt(value);
                    break;
                case "minimumcellvoltage":
                    MinimumCellVoltage = ToDouble(value);
                    break;
                case "maximumcellvoltage":
                    MaximumCellVoltage = ToDouble(value);
                    break;
                case "chargedenergy":
                    ChargedEnergy = ToDouble(value);
                    break;
                case "dischargedenergy":
                    DischargedEnergy = ToDouble(value);
                    break;
            }
        }
    }

    private void Seed()
    {
        // Seed all known paths with null so every path is registered on D-Bus at startup.
        // Venus OS treats a variant of type "i" value 0 as "no valid data" for numeric paths.
        SetValue(DbusPath.Soc, null);
        SetValue(DbusPath.Voltage, null);
        SetValue(DbusPath.Current, null);
        SetValue(DbusPath.Power, null);
        SetValue(DbusPath.Temperature, null);
        SetValue(DbusPath.Capacity, null);
        SetValue(DbusPath.InstalledCapacity, null);
        SetValue(DbusPath.TimeToGo, null);
        SetValue(DbusPath.State, null);
        SetValue(DbusPath.AlarmLowVoltage, null);
        SetValue(DbusPath.AlarmHighVoltage, null);
        SetValue(DbusPath.AlarmLowSoc, null);
        SetValue(DbusPath.AlarmLowTemperature, null);
        SetValue(DbusPath.AlarmHighTemperature, null);
        SetValue(DbusPath.MinimumCellVoltage, null);
        SetValue(DbusPath.MaximumCellVoltage, null);
        SetValue(DbusPath.ChargedEnergy, null);
        SetValue(DbusPath.DischargedEnergy, null);

        // Identity paths — always valid from configuration
        ProductName = _options.ProductName;
        DeviceInstance = _options.DeviceInstance;
        Connected = 0;
    }

    private static double? ToDouble(object? value)
    {
        if (value is null) return null;
        return value is double d ? d : Convert.ToDouble(value, System.Globalization.CultureInfo.InvariantCulture);
    }

    private static int? ToInt(object? value)
    {
        if (value is null) return null;
        return value is int i ? i : Convert.ToInt32(value, System.Globalization.CultureInfo.InvariantCulture);
    }

    internal static class DbusPath
    {
        public const string Soc = "/Soc";
        public const string Voltage = "/Dc/0/Voltage";
        public const string Current = "/Dc/0/Current";
        public const string Power = "/Dc/0/Power";
        public const string Temperature = "/Dc/0/Temperature";
        public const string Capacity = "/Capacity";
        public const string InstalledCapacity = "/InstalledCapacity";
        public const string TimeToGo = "/TimeToGo";
        public const string Connected = "/Connected";
        public const string State = "/State";
        public const string ProductName = "/ProductName";
        public const string DeviceInstance = "/DeviceInstance";
        public const string AlarmLowVoltage = "/Alarms/LowVoltage";
        public const string AlarmHighVoltage = "/Alarms/HighVoltage";
        public const string AlarmLowSoc = "/Alarms/LowSoc";
        public const string AlarmLowTemperature = "/Alarms/LowTemperature";
        public const string AlarmHighTemperature = "/Alarms/HighTemperature";
        public const string MinimumCellVoltage = "/History/MinimumCellVoltage";
        public const string MaximumCellVoltage = "/History/MaximumCellVoltage";
        public const string ChargedEnergy = "/History/ChargedEnergy";
        public const string DischargedEnergy = "/History/DischargedEnergy";
    }
}

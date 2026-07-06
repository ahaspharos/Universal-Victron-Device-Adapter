using Microsoft.Extensions.Options;
using VictronBridge.Configuration;
using VictronBridge.Models;

namespace VictronBridge.Devices.PvInverter;

public sealed class PvInverterDeviceModel : DeviceModelBase
{
    private readonly DeviceOptions _options;

    public PvInverterDeviceModel(IOptions<DeviceOptions> options)
    {
        _options = options.Value;
    }

    public override string ServiceName => _options.ServiceName;
    public override string DeviceType => "com.victronenergy.pvinverter";

    public double? L1Power
    {
        get => GetValue(DbusPath.L1Power) as double?;
        set => SetValue(DbusPath.L1Power, value);
    }

    public double? L2Power
    {
        get => GetValue(DbusPath.L2Power) as double?;
        set => SetValue(DbusPath.L2Power, value);
    }

    public double? L3Power
    {
        get => GetValue(DbusPath.L3Power) as double?;
        set => SetValue(DbusPath.L3Power, value);
    }

    public double? L1Voltage
    {
        get => GetValue(DbusPath.L1Voltage) as double?;
        set => SetValue(DbusPath.L1Voltage, value);
    }

    public double? L1Current
    {
        get => GetValue(DbusPath.L1Current) as double?;
        set => SetValue(DbusPath.L1Current, value);
    }

    public double? TotalEnergy
    {
        get => GetValue(DbusPath.TotalEnergy) as double?;
        set => SetValue(DbusPath.TotalEnergy, value);
    }

    public string? ProductName
    {
        get => GetValue(DbusPath.ProductName) as string;
        set => SetValue(DbusPath.ProductName, value);
    }

    public override void ApplyValues(IReadOnlyDictionary<string, object?> values)
    {
        foreach (var (key, value) in values)
        {
            switch (key.ToLowerInvariant())
            {
                case "l1power":
                    L1Power = Convert.ToDouble(value);
                    break;
                case "l2power":
                    L2Power = Convert.ToDouble(value);
                    break;
                case "l3power":
                    L3Power = Convert.ToDouble(value);
                    break;
                case "l1voltage":
                    L1Voltage = Convert.ToDouble(value);
                    break;
                case "l1current":
                    L1Current = Convert.ToDouble(value);
                    break;
                case "totalenergy":
                    TotalEnergy = Convert.ToDouble(value);
                    break;
                case "productname":
                    ProductName = value?.ToString();
                    break;
            }
        }
    }

    private static class DbusPath
    {
        public const string L1Power = "/Ac/L1/Power";
        public const string L2Power = "/Ac/L2/Power";
        public const string L3Power = "/Ac/L3/Power";
        public const string L1Voltage = "/Ac/L1/Voltage";
        public const string L1Current = "/Ac/L1/Current";
        public const string TotalEnergy = "/Ac/Energy/Forward";
        public const string ProductName = "/ProductName";
    }
}

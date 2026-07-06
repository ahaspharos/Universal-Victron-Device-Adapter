using VictronBridge.DBus;

namespace VictronBridge.Tests;

public sealed class VictronBusItemHandlerTests
{
    [Fact]
    public void Constructor_SetsPath()
    {
        var handler = new VictronBusItemHandler("/Soc", "com.victronenergy.battery.test", 80.0);
        Assert.Equal("/Soc", handler.Path);
    }

    [Fact]
    public void Constructor_SetsInitialValue()
    {
        var handler = new VictronBusItemHandler("/Soc", "com.victronenergy.battery.test", 80.0);
        Assert.Equal(80.0, handler.CurrentValue);
    }

    [Fact]
    public void HandlesChildPaths_ReturnsFalse()
    {
        var handler = new VictronBusItemHandler("/Soc", "com.victronenergy.battery.test", null);
        Assert.False(handler.HandlesChildPaths);
    }

    [Fact]
    public void UpdateValue_ChangesCurrentValue()
    {
        var handler = new VictronBusItemHandler("/Soc", "com.victronenergy.battery.test", 80.0);
        handler.UpdateValue(95.0);
        Assert.Equal(95.0, handler.CurrentValue);
    }

    [Fact]
    public void UpdateValue_ToNull_CurrentValueIsNull()
    {
        var handler = new VictronBusItemHandler("/Soc", "com.victronenergy.battery.test", 80.0);
        handler.UpdateValue(null);
        Assert.Null(handler.CurrentValue);
    }
}

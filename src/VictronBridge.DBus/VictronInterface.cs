namespace VictronBridge.DBus;

internal static class VictronInterface
{
    public const string BusItem = "com.victronenergy.BusItem";

    public const string GetValue = "GetValue";
    public const string GetText = "GetText";
    public const string SetValue = "SetValue";
    public const string PropertiesChanged = "PropertiesChanged";

    // org.freedesktop.DBus standard interfaces
    public const string Properties = "org.freedesktop.DBus.Properties";
    public const string Introspectable = "org.freedesktop.DBus.Introspectable";
    public const string Peer = "org.freedesktop.DBus.Peer";

    public static readonly ReadOnlyMemory<byte> BusItemIntrospectionXml =
        System.Text.Encoding.UTF8.GetBytes("""
            <interface name="com.victronenergy.BusItem">
              <signal name="PropertiesChanged">
                <arg name="properties" type="a{sv}"/>
              </signal>
              <method name="GetValue">
                <arg direction="out" type="v" name="value"/>
              </method>
              <method name="GetText">
                <arg direction="out" type="s" name="text"/>
              </method>
              <method name="SetValue">
                <arg direction="in"  type="v" name="value"/>
                <arg direction="out" type="i" name="result"/>
              </method>
            </interface>
            """);
}

using Tmds.DBus.Protocol;

namespace VictronBridge.DBus;

internal sealed class VictronBusItemHandler : IPathMethodHandler
{
    private readonly string _path;
    private readonly string _serviceName;
    private object? _value;

    public string Path => _path;
    public bool HandlesChildPaths => false;

    public VictronBusItemHandler(string path, string serviceName, object? initialValue)
    {
        _path = path;
        _serviceName = serviceName;
        _value = initialValue;
    }

    public void UpdateValue(object? value) => _value = value;

    public object? CurrentValue => _value;

    public ValueTask HandleMethodAsync(MethodContext context)
    {
        var request = context.Request;

        if (context.IsDBusIntrospectRequest)
        {
            context.ReplyIntrospectXml(
                new[] { VictronInterface.BusItemIntrospectionXml },
                Array.Empty<string>());
            return ValueTask.CompletedTask;
        }

        var iface = request.InterfaceAsString;
        var member = request.MemberAsString;

        if (iface == VictronInterface.BusItem || iface is null)
        {
            if (member == VictronInterface.GetValue)
            {
                ReplyGetValue(context);
                return ValueTask.CompletedTask;
            }
            if (member == VictronInterface.GetText)
            {
                ReplyGetText(context);
                return ValueTask.CompletedTask;
            }
            if (member == VictronInterface.SetValue)
            {
                ReplySetValue(context);
                return ValueTask.CompletedTask;
            }
        }

        context.ReplyUnknownMethodError();
        return ValueTask.CompletedTask;
    }

    private void ReplyGetValue(MethodContext context)
    {
        var writer = context.CreateReplyWriter("v");
        WriteVariantValue(ref writer, _value);
        context.Reply(writer.CreateMessage());
        writer.Dispose();
    }

    private void ReplyGetText(MethodContext context)
    {
        using var writer = context.CreateReplyWriter("s");
        writer.WriteString(_value?.ToString() ?? string.Empty);
        context.Reply(writer.CreateMessage());
    }

    private void ReplySetValue(MethodContext context)
    {
        // Venus OS sets values via D-Bus; we accept but don't propagate upstream for now
        using var writer = context.CreateReplyWriter("i");
        writer.WriteInt32(0);
        context.Reply(writer.CreateMessage());
    }

    internal static void WriteVariantValue(ref MessageWriter writer, object? value)
    {
        switch (value)
        {
            case null:
                // Venus OS convention: invalid/unavailable value is represented as an empty variant (type "i", value 0 invalid)
                // Commonly used: write variant with signature "i" and value 0 to signal "no valid data"
                writer.WriteVariantInt32(0);
                break;
            case double d:
                writer.WriteVariantDouble(d);
                break;
            case float f:
                writer.WriteVariantDouble(f);
                break;
            case int i:
                writer.WriteVariantInt32(i);
                break;
            case long l:
                writer.WriteVariantInt64(l);
                break;
            case bool b:
                writer.WriteVariantBool(b);
                break;
            case string s:
                writer.WriteVariantString(s);
                break;
            default:
                writer.WriteVariantString(value.ToString() ?? string.Empty);
                break;
        }
    }

    internal MessageBuffer CreatePropertiesChangedSignal(DBusConnection connection)
    {
        var writer = connection.GetMessageWriter();
        // destination must be null for broadcast signals — passing the service name
        // unicasts the signal to ourselves and Venus OS never receives it.
        writer.WriteSignalHeader(
            destination: null,
            _path,
            VictronInterface.BusItem,
            VictronInterface.PropertiesChanged,
            "a{sv}");

        // Write a{sv}: one entry: "Value" => variant
        var arrayStart = writer.WriteDictionaryStart();
        writer.WriteDictionaryEntryStart();
        writer.WriteString("Value");
        WriteVariantValue(ref writer, _value);
        writer.WriteDictionaryEnd(arrayStart);

        return writer.CreateMessage();
    }
}

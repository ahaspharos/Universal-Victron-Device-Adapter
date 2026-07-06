using Tmds.DBus.Protocol;

namespace VictronBridge.DBus;

internal sealed class VictronRootHandler : IPathMethodHandler
{
    private readonly string _serviceName;
    private readonly Func<IReadOnlyDictionary<string, object?>> _getValues;

    public string Path => "/";
    public bool HandlesChildPaths => false;

    public VictronRootHandler(
        string serviceName,
        Func<IReadOnlyDictionary<string, object?>> getValues)
    {
        _serviceName = serviceName;
        _getValues = getValues;
    }

    public ValueTask HandleMethodAsync(MethodContext context)
    {
        var request = context.Request;

        if (context.IsDBusIntrospectRequest)
        {
            var childPaths = _getValues().Keys
                .Select(k => k.TrimStart('/').Split('/')[0])
                .Distinct()
                .ToList();

            context.ReplyIntrospectXml(
                new[] { VictronInterface.BusItemIntrospectionXml },
                childPaths);
            return ValueTask.CompletedTask;
        }

        var member = request.MemberAsString;

        if (member == VictronInterface.GetValue)
        {
            ReplyGetAllValues(context);
            return ValueTask.CompletedTask;
        }

        context.ReplyUnknownMethodError();
        return ValueTask.CompletedTask;
    }

    private void ReplyGetAllValues(MethodContext context)
    {
        // Venus OS root GetValue returns a{sv}: path → variant value
        var writer = context.CreateReplyWriter("a{sv}");

        var values = _getValues();
        var arrayStart = writer.WriteDictionaryStart();
        foreach (var (path, value) in values)
        {
            writer.WriteDictionaryEntryStart();
            writer.WriteString(path);
            VictronBusItemHandler.WriteVariantValue(ref writer, value);
        }
        writer.WriteDictionaryEnd(arrayStart);

        context.Reply(writer.CreateMessage());
        writer.Dispose();
    }
}

using System.Reflection;
using Newtonsoft.Json.Linq;

namespace Raid.Toolkit.AccountExtension;

internal class DiscoveryHandler : IApiServer<SocketMessage>
{
    private string Name => "$router/discover";

    private readonly List<string> ServiceNames = new();

    public DiscoveryHandler(IReadOnlyList<IApiServer<SocketMessage>> services)
    {
        foreach (var service in services)
        {
            foreach (var attr in service.GetType().GetInterfaces().Select(type => type.GetCustomAttribute<PublicApiAttribute>(true)))
            {
                if (attr != null)
                    ServiceNames.Add(attr.Name);
            }
        }
    }

    private string[] Types => ServiceNames.ToArray();

    public bool SupportsScope(string scopeName)
    {
        return scopeName == Name;
    }

    public void HandleMessage(SocketMessage message, IApiSession<SocketMessage> session)
    {
        switch (message.Channel)
        {
            case "request":
                session.SendAsync(new SocketMessage(Name, "response", JArray.FromObject(Types)));
                break;
            case "response":
                break;
        }
    }
}

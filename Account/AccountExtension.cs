using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

using Newtonsoft.Json;

using Raid.Toolkit.Extensibility.Services;

using SuperSocket.WebSocket;
using SuperSocket.WebSocket.Server;

using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace Raid.Toolkit.AccountExtension;


public class SuperSocketAdapter : IApiSession<SocketMessage>
{
	private readonly WebSocketSession Session;
	public SuperSocketAdapter(WebSocketSession session) => Session = session;

	public string Id => Session.SessionID;
	public bool Connected => Session.State == SuperSocket.SessionState.Connected;

	public async Task SendAsync(SocketMessage message, CancellationToken token = default)
	{
		try
		{
			await Session.SendAsync(JsonConvert.SerializeObject(message));
		}
		catch (Exception)
		{ }
	}
}

public class AccountExtension : ExtensionPackage, IDisposable
{
	private IHost? ServerHost;
	private readonly List<IApiServer<SocketMessage>> Servers = new();
	public AccountExtension(ILogger<AccountExtension> logger)
	{
	}

	public override void OnActivate(IExtensionHost host)
	{
		Servers.Add(host.CreateInstance<StaticDataApi>(host));
		Servers.Add(host.CreateInstance<AccountApi>(host));
		Servers.Add(host.CreateInstance<RealtimeApi>(host));
		Servers.Add(host.CreateInstance<DiscoveryHandler>(Servers));

		Disposables.Add(host.RegisterAccountExtension(new AccountDataExtensionFactory<RealtimeExtension>(host, true)));

		Disposables.Add(host.RegisterAccountExtension(new AccountDataExtensionFactory<StaticTypesExtension>(host, true)));

		Disposables.Add(host.RegisterAccountExtension(new AccountDataExtensionFactory<ArtifactExtension>(host, false)));
		Disposables.Add(host.RegisterAccountExtension(new AccountDataExtensionFactory<HeroesExtension>(host, false)));
		Disposables.Add(host.RegisterAccountExtension(new AccountDataExtensionFactory<ArenaExtension>(host, false)));
		Disposables.Add(host.RegisterAccountExtension(new AccountDataExtensionFactory<AcademyExtension>(host, false)));
		Disposables.Add(host.RegisterAccountExtension(new AccountDataExtensionFactory<ResourcesExtension>(host, false)));

		ServerHost = WebSocketHostBuilder.Create().
			UseWebSocketMessageHandler(HandleMessage)
			.ConfigureAppConfiguration((hostCtx, configApp) =>
			{
				configApp.AddInMemoryCollection(new Dictionary<string, string>
				{
					{ "serverOptions:name", "raid-toolkit-service" },
					{ "serverOptions:listeners:0:ip", "Any" },
					{ "serverOptions:listeners:0:port", "9090" }
				});
			}).Build();
		ServerHost.Start();
	}

	private ValueTask HandleMessage(WebSocketSession session, WebSocketPackage package)
	{
		SuperSocketAdapter adapter = new(session);
		SocketMessage? socketMessage = JsonConvert.DeserializeObject<SocketMessage>(package.Message);
		if (socketMessage == null)
			return ValueTask.CompletedTask;

		foreach (var server in Servers.Where(server => server.SupportsScope(socketMessage.Scope)))
		{
			server.HandleMessage(socketMessage, adapter);
		}
		return ValueTask.CompletedTask;
	}

	public override void OnDeactivate(IExtensionHost host)
	{
		base.OnDeactivate(host);
		ServerHost?.StopAsync().Wait();
	}
}

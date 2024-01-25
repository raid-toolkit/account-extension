namespace Raid.Toolkit.AccountExtension;

public class RealtimeApi : ApiServer<IRealtimeApi>, IRealtimeApi
{
	public static RealtimeApi? Singleton {  get; private set; }

	private readonly IGameInstanceManager InstanceManager;
	private readonly IExtensionHost Host;

	public event EventHandler<EmptyEventArgs>? AccountListUpdated;
	public event EventHandler<ViewUpdatedEventArgs>? ViewChanged;
	public event EventHandler<AccountUpdatedEventArgs>? ReceiveBattleResponse;

	public bool Enabled { get; set; }

	public RealtimeApi(ILogger<RealtimeApi> logger, IGameInstanceManager instanceManager, IExtensionHost host)
		: base(logger)
	{
		Singleton = this;
		Host = host;
		InstanceManager = instanceManager;
		InstanceManager.OnAdded += (sender, _) => HandleInstanceManagerUpdateEvent(sender);
		InstanceManager.OnRemoved += (sender, _) => HandleInstanceManagerUpdateEvent(sender);
	}

	internal void OnViewChanged(ViewChangedEventArgs e)
	{
		ViewChanged?.Raise(this, new ViewUpdatedEventArgs(e.Id, e.ViewMeta.Key.ToString()));
	}

	internal void OnBattleResultChanged(BattleResultsChangedEventArgs e)
	{
		ReceiveBattleResponse?.Raise(this, new AccountUpdatedEventArgs(e.Id));
	}

	private void HandleInstanceManagerUpdateEvent(object? sender)
	{
		AccountListUpdated?.Raise(this, new EmptyEventArgs());
	}

	public Task<Account[]> GetConnectedAccounts()
	{
		Enabled = true; // enable on first use
		return Task.FromResult(
			Host.GetAccounts()
				.Where(account => account.IsOnline)
				.Select(account => Account.FromBase(account.AccountInfo, DateTime.UtcNow))
				.ToArray());
	}

	public Task<ViewInfo> GetCurrentViewInfo(string accountId)
	{
		Enabled = true; // enable on first use
		RealtimeDataSnapshot data = GetRealtimeSnapshot(accountId);

		return Task.FromResult(
			new ViewInfo()
			{
				ViewId = (int)data.View,
				ViewKey = data.View.ToString()
			});
	}
	public Task<LastBattleDataObject> GetLastBattleResponse(string accountId)
	{
		Enabled = true; // enable on first use
		RealtimeDataSnapshot data = GetRealtimeSnapshot(accountId);

		return Task.FromResult(data.LastBattle!);
	}

	private RealtimeDataSnapshot GetRealtimeSnapshot(string accountId)
	{
		if (!InstanceManager.TryGetById(accountId, out ILoadedGameInstance? gameInstance))
			throw new ArgumentOutOfRangeException(nameof(accountId));

		if (!Host.TryGetAccount(accountId, out IAccount? account) ||
			!account.TryGetApi(out IGetAccountDataApi<RealtimeDataSnapshot>? api) ||
			!api.TryGetData(out RealtimeDataSnapshot? data))
			throw new KeyNotFoundException(accountId);
		return data;
	}
}

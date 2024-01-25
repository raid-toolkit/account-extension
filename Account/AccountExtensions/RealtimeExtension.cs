using Client.RaidApp;
using Client.ViewModel.DTO;

using Raid.Toolkit.Extension;

namespace Raid.Toolkit.AccountExtension;

public class RealtimeDataSnapshot
{
	public ViewKey View = ViewKey.None;
	public DateTime? LastBattleResponse;
	public LastBattleDataObject? LastBattle;
}

public class ViewChangedEventArgs : EventArgs
{
	public string Id { get; }
	public ViewMeta ViewMeta { get; }
	public ViewChangedEventArgs(string id, ViewMeta viewMeta)
	{
		Id = id;
		ViewMeta = viewMeta;
	}
}

public class BattleResultsChangedEventArgs : EventArgs
{
	public string Id { get; }
	public RealtimeDataSnapshot Snapshot { get; }
	public BattleResultsChangedEventArgs(string id, RealtimeDataSnapshot snapshot)
	{
		Id = id;
		Snapshot = snapshot;
	}
}

public class RealtimeExtension : AccountDataExtensionBase,
	IAccountPublicApi<IGetAccountDataApi<RealtimeDataSnapshot>>,
	IGetAccountDataApi<RealtimeDataSnapshot>
{
	private const string Key = "heroes.json";

	IGetAccountDataApi<RealtimeDataSnapshot> IAccountPublicApi<IGetAccountDataApi<RealtimeDataSnapshot>>.GetApi() => this;
	bool IGetAccountDataApi<RealtimeDataSnapshot>.TryGetData([NotNullWhen(true)] out RealtimeDataSnapshot? data) => Storage.TryRead(Key, out data);

	private readonly RealtimeDataSnapshot Snapshot = new();

	public RealtimeExtension(IAccount account, IExtensionStorage storage, ILogger logger)
		: base(account, storage, logger)
	{
	}

	protected override Task Update(ModelScope scope)
	{
		UpdateLastView(scope);
		UpdateLastBattleState(scope);
		return Task.CompletedTask;
	}

	private void UpdateLastView(ModelScope scope)
	{
		if (scope.RaidApplication._viewMaster is not RaidViewMaster viewMaster)
			return;

		if (viewMaster._views.Count == 0)
			return;

		ViewMeta topView = viewMaster._views[^1];
		if (Snapshot.View != topView.Key)
		{
			Snapshot.View = topView.Key;
			RealtimeApi.Singleton?.OnViewChanged(new(Account.Id, topView));
			return;
		}
	}

	private void UpdateLastBattleState(ModelScope scope)
	{
		var response = scope.AppModel._userWrapper.Battle.BattleData.LastResponse;
		if (Snapshot.LastBattleResponse == response.StartTime)
			return;

		Snapshot.LastBattleResponse = response.StartTime;
		Snapshot.LastBattle = (new()
		{
			BattleKindId = response.BattleKindId.ToString(),
			HeroesExperience = response.HeroesExperience,
			HeroesExperienceAdded = response.HeroesExperienceAdded,
			Turns = response.Turns,
			TournamentPointsByStateId = response.TournamentPointsByStateId.UnderlyingDictionary,
			GivenDamage = new()
			{
				DemonLord = response.GivenDamageToAllianceBoss,
				Hydra = response.GivenDamageToAllianceHydra,
			},
			MasteryPointsByHeroId = response.MasteryPointsByHeroId?.ToDictionary(
				kvp => kvp.Key,
				kvp => (Dictionary<string, int>)kvp.Value.UnderlyingDictionary.ToModel())
		});
		RealtimeApi.Singleton?.OnBattleResultChanged(new(Account.Id, Snapshot));
	}
}

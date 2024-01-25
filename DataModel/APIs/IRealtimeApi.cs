using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Raid.Toolkit.Common.API;
using Raid.Toolkit.Common.API.Messages;

namespace Raid.Toolkit.DataModel;

public class ViewUpdatedEventArgs : SerializableEventArgs
{
	public string AccountId => (string)EventArguments[0];
	public string ViewId => (string)EventArguments[1];

	public ViewUpdatedEventArgs(string accountId, string viewId)
		: base(accountId, viewId)
	{
	}

}

public class AccountUpdatedEventArgs : SerializableEventArgs
{
	public string AccountId => (string)EventArguments[0];

	public AccountUpdatedEventArgs(string viewId)
		: base(viewId)
	{
	}

}

public class EmptyEventArgs : SerializableEventArgs
{
	public EmptyEventArgs() : base(Array.Empty<object>())
	{ }
}

[PublicApi("realtime-api")]
public interface IRealtimeApi
{
	[PublicApi("account-list-updated")]
	event EventHandler<EmptyEventArgs> AccountListUpdated;

	[PublicApi("view-changed")]
	event EventHandler<ViewUpdatedEventArgs> ViewChanged;

	[PublicApi("last-battle-response-updated")]
	event EventHandler<AccountUpdatedEventArgs> ReceiveBattleResponse;

	[PublicApi("getConnectedAccounts")]
	Task<Account[]> GetConnectedAccounts();

	[PublicApi("getLastBattleResponse")]
	Task<LastBattleDataObject> GetLastBattleResponse(string accountId);

	[PublicApi("getCurrentViewInfo")]
	Task<ViewInfo> GetCurrentViewInfo(string accountId);
}

public class ViewInfo
{
	[JsonProperty("viewId")]
	public int? ViewId;

	[JsonProperty("viewKey")]
	public string? ViewKey;
}

public class GivenDamage
{
	[JsonProperty("demonLord")]
	public long? DemonLord;
	[JsonProperty("hydra")]
	public long? Hydra;
}

public class LastBattleDataObject
{
	[JsonProperty("battleKindId")]
	public string? BattleKindId;

	[JsonProperty("heroesExperience")]
	public int HeroesExperience;

	[JsonProperty("heroesExperienceAdded")]
	public int HeroesExperienceAdded;

	[JsonProperty("turnCount")]
	public int? Turns;

	[JsonProperty("givenDamage")]
	public GivenDamage? GivenDamage;

	[JsonProperty("tournamentPointsByStateId")]
	public Dictionary<int, int>? TournamentPointsByStateId;

	[JsonProperty("masteryPointsByHeroId")]
	public Dictionary<int, Dictionary<string, int>>? MasteryPointsByHeroId;
}

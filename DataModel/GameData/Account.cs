using System;

using Newtonsoft.Json;

using Raid.Toolkit.Extensibility;

namespace Raid.Toolkit.DataModel;

public class Account : AccountBase
{
	public Account(string id, string avatar, string avatarId, string name, int level, int power) : base(id, avatar, avatarId, name, level, power)
	{
	}

	[JsonProperty("lastUpdated")]
	public DateTime? LastUpdated;

	public static Account FromBase(AccountBase accountBase, DateTime? lastUpdated)
	{
		return new(accountBase.Id, accountBase.Avatar, accountBase.AvatarId, accountBase.Name, accountBase.Level, accountBase.Power);
	}
}

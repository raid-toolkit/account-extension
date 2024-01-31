using System;
using System.Reflection;
using System.Threading.Tasks;

using Raid.Toolkit.Common.API;
using Raid.Toolkit.Common.API.Messages;
using Raid.Toolkit.DataModel;

namespace Raid.Client
{
	public class AccountApi : ApiCallerBase<IAccountApi>, IAccountApi
	{
		public AccountApi(ApiClientBase client) : base(client) { }

		public event EventHandler<SerializableEventArgs> Updated
		{
			add { AddHandler(nameof(Updated), value); }
			remove { RemoveHandler(nameof(Updated), value); }
		}

		public Task<Account> GetAccount(string accountId)
		{
			return CallMethod<Account>(nameof(GetAccount), accountId);
		}

		public Task<RaidExtractor.Core.AccountDump> GetAccountDump(string accountId)
		{
			return CallMethod<RaidExtractor.Core.AccountDump>(nameof(GetAccountDump), accountId);
		}

		public Task<Account[]> GetAccounts()
		{
			return CallMethod<Account[]>(nameof(GetAccounts));
		}

		public Task<Resources> GetAllResources(string accountId)
		{
			return CallMethod<Resources>(nameof(GetAllResources), accountId);
		}

		public Task<Artifact> GetArtifactById(string accountId, int artifactId)
		{
			return CallMethod<Artifact>(nameof(GetArtifactById), accountId, artifactId);
		}

		public Task<Artifact[]> GetArtifacts(string accountId)
		{
			return CallMethod<Artifact[]>(nameof(GetArtifacts), accountId);
		}

		public Task<Hero> GetHeroById(string accountId, int heroId, bool snapshot = false)
		{
			return CallMethod<Hero>(nameof(GetHeroById), accountId, heroId, snapshot);
		}

		public Task<Hero[]> GetHeroes(string accountId, bool snapshot = false)
		{
			return CallMethod<Hero[]>(nameof(GetHeroes), accountId, snapshot);
		}

		public Task<ArenaData> GetArena(string accountId)
		{
			return CallMethod<ArenaData>(nameof(GetArena), accountId);
		}

		public Task<AcademyData> GetAcademy(string accountId)
		{
			return CallMethod<AcademyData>(nameof(GetAcademy), accountId);
		}
	}
}

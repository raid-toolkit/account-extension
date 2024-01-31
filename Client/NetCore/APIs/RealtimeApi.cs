using System;
using System.Reflection;
using System.Threading.Tasks;

using GitHub;

using Raid.Toolkit.Common.API;
using Raid.Toolkit.Common.API.Messages;
using Raid.Toolkit.DataModel;

namespace Raid.Client
{
	public class RealtimeApi : ApiCallerBase<IRealtimeApi>, IRealtimeApi
	{
		public RealtimeApi(ApiClientBase client) : base(client) { }

		public event EventHandler<EmptyEventArgs> AccountListUpdated
		{
			add { AddHandler(nameof(AccountListUpdated), value); }
			remove { RemoveHandler(nameof(AccountListUpdated), value); }
		}
		public event EventHandler<ViewUpdatedEventArgs> ViewChanged
		{
			add { AddHandler(nameof(ViewChanged), value); }
			remove { RemoveHandler(nameof(ViewChanged), value); }
		}
		public event EventHandler<AccountUpdatedEventArgs> ReceiveBattleResponse
		{
			add { AddHandler(nameof(ReceiveBattleResponse), value); }
			remove { RemoveHandler(nameof(ReceiveBattleResponse), value); }
		}

		public Task<Account[]> GetConnectedAccounts()
		{
			return CallMethod<Account[]>(nameof(GetConnectedAccounts));
		}

		public Task<ViewInfo> GetCurrentViewInfo(string accountId)
		{
			return CallMethod<ViewInfo>(nameof(GetCurrentViewInfo), accountId);
		}

		public Task<LastBattleDataObject> GetLastBattleResponse(string accountId)
		{
			return CallMethod<LastBattleDataObject>(nameof(GetLastBattleResponse), accountId);
		}
	}
}

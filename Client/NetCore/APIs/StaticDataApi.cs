using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

using Raid.Toolkit.Common.API;
using Raid.Toolkit.DataModel;

namespace Raid.Client
{
	public class StaticDataApi : ApiCallerBase<IStaticDataApi>, IStaticDataApi
	{
		public StaticDataApi(ApiClientBase client) : base(client) { }

		public Task<StaticData> GetAllData()
		{
			return CallMethod<StaticData>(nameof(GetAllData));
		}

		public Task<StaticArenaData> GetArenaData()
		{
			return CallMethod<StaticArenaData>(nameof(GetArenaData));
		}

		public Task<StaticArtifactData> GetArtifactData()
		{
			return CallMethod<StaticArtifactData>(nameof(GetArtifactData));
		}

		public Task<StaticHeroTypeData> GetHeroData()
		{
			return CallMethod<StaticHeroTypeData>(nameof(GetHeroData));
		}

		public Task<IReadOnlyDictionary<string, string>> GetLocalizedStrings()
		{
			return CallMethod<IReadOnlyDictionary<string, string>>(nameof(GetLocalizedStrings));
		}

		public Task<StaticSkillData> GetSkillData()
		{
			return CallMethod<StaticSkillData>(nameof(GetSkillData));
		}

		public Task<StaticStageData> GetStageData()
		{
			return CallMethod<StaticStageData>(nameof(GetStageData));
		}
	}
}

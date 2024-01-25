using Client.Model;
using Client.Model.Gameplay.StaticData;
using Client.RaidApp;

using Il2CppToolkit.Runtime;

namespace Raid.Toolkit.AccountExtension;

public class ModelScope
{
	public Il2CsRuntimeContext Context { get; }

	private AppModel? _AppModel;
	public AppModel AppModel => _AppModel ??= Client.App.SingleInstance<AppModel>._instance.GetValue(Context);

	private ClientStaticDataManager? _StaticDataManager;
	public ClientStaticDataManager StaticDataManager => _StaticDataManager ??= AppModel.StaticDataManager as ClientStaticDataManager ?? throw new InvalidCastException();

	private RaidApplication? _RaidApplication;
	public RaidApplication RaidApplication => _RaidApplication ??= Client.App.Application._instance.GetValue(Context) as RaidApplication ?? throw new InvalidCastException();

	public ModelScope(Il2CsRuntimeContext context)
	{
		Context = context;
	}
}

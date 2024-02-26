using Nakuru.Unity.Ecs.Utilities;
using Unity.Entities;

namespace MatchX.Client.Presentation
{

	public partial class HybridPresentationSystemGroup : StrictOrderSystemsGroup
	{
		protected override void OnCreate()
		{
			base.OnCreate();
			
			AddSystemToUpdateList(World.CreateSystem<DetectTransformChangesSystem>());
			AddSystemToUpdateList(World.CreateSystem<NewGameObjectCreationSystem>());
			AddSystemToUpdateList(World.CreateSystem<AddressableAssetLoadingSystem>());
			AddSystemToUpdateList(World.CreateSystem<GameObjectParentSystem>());
			AddSystemToUpdateList(World.CreateSystem<SyncGameObjectTransformSystem>());
		}
	}

}
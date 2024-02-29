using Unity.Entities;

namespace Nakuru.Entities.Hybrid.Presentation
{

	[UpdateInGroup(typeof(PresentationSystemGroup), OrderFirst = true)]
	public partial class HybridPresentationSystemGroup : ComponentSystemGroup
	{
		public HybridPresentationSystemGroup() { EnableSystemSorting = false; }

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
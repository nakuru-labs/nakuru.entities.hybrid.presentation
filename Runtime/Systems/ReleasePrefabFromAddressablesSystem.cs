using Unity.Collections;
using Unity.Entities;
using UnityEngine.AddressableAssets;

namespace Nakuru.Entities.Hybrid.Presentation
{

	[RequireMatchingQueriesForUpdate]
	public partial struct ReleasePrefabFromAddressablesSystem : ISystem
	{
		public void OnUpdate(ref SystemState state)
		{
			var ecb = new EntityCommandBuffer(Allocator.Temp);

			foreach (var (prefabRef, entity) in SystemAPI.Query<PrefabRef>()
			                                             .WithAll<PrefabOriginAddressables>()
			                                             .WithNone<ViewElement.Tag>()
			                                             .WithEntityAccess()) {
				Addressables.Release(prefabRef.Value);

				ecb.RemoveComponent<PrefabRef>(entity);
				ecb.RemoveComponent<PrefabOriginAddressables>(entity);
			}

			ecb.Playback(state.EntityManager);
		}
	}

}
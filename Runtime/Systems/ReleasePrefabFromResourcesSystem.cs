using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Nakuru.Entities.Hybrid.Presentation
{

	[RequireMatchingQueriesForUpdate]
	public partial struct ReleasePrefabFromResourcesSystem : ISystem
	{
		public void OnUpdate(ref SystemState state)
		{
			var ecb = new EntityCommandBuffer(Allocator.Temp);

			foreach (var (prefabRef, entity) in SystemAPI.Query<PrefabRef>()
			                                             .WithAll<PrefabOriginResources>()
			                                             .WithNone<ViewElement.Tag>()
			                                             .WithEntityAccess()) {
				
				Object.Destroy(prefabRef.Value);

				ecb.RemoveComponent<PrefabRef>(entity);
				ecb.RemoveComponent<PrefabOriginResources>(entity);
			}

			ecb.Playback(state.EntityManager);
		}
	}

}
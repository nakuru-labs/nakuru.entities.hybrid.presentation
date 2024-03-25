using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Nakuru.Entities.Hybrid.Presentation
{

	[DisableAutoCreation]
	[RequireMatchingQueriesForUpdate]
	public partial struct InstantiatePrefabSystem : ISystem
	{
		[BurstCompile]
		public void OnCreate(ref SystemState state) { }

		public void OnUpdate(ref SystemState state)
		{
			var ecb = new EntityCommandBuffer(Allocator.Temp);

			foreach (var (prefabRef, entity) in SystemAPI.Query<PrefabRef>()
			                                             .WithNone<GameObjectRef>()
			                                             .WithEntityAccess()) {
				var instance = Object.Instantiate(prefabRef.Value);
				ecb.AddComponent(entity, new GameObjectRef { Value = instance });
				ecb.AddComponent<ViewElement.Event.OnBorn>(entity);
			}

			ecb.Playback(state.EntityManager);
		}
	}

}
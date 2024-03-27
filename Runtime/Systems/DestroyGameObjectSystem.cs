using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Nakuru.Entities.Hybrid.Presentation
{

	[DisableAutoCreation]
	public partial struct DestroyGameObjectSystem : ISystem
	{
		[BurstCompile]
		public void OnCreate(ref SystemState state)
		{ }
		
		public void OnUpdate(ref SystemState state)
		{
			var ecb = new EntityCommandBuffer(Allocator.Temp);
			
			foreach (var (gameObjectRef, entity) in SystemAPI.Query<GameObjectRef>()
			                                         .WithNone<ViewElement.Tag>()
			                                         .WithEntityAccess()) {
				Object.Destroy(gameObjectRef.Value);
				ecb.RemoveComponent<GameObjectRef>(entity);
			}
			
			ecb.Playback(state.EntityManager);
		}

		[BurstCompile]
		public void OnDestroy(ref SystemState state) { }
	}

}
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Nakuru.Entities.Hybrid.Presentation
{

	[DisableAutoCreation]
	[RequireMatchingQueriesForUpdate]
	public partial struct NewGameObjectCreationSystem : ISystem
	{
		public void OnUpdate(ref SystemState state)
		{
			var ecb = new EntityCommandBuffer(Allocator.Temp);
			
			foreach (var (_, entity) in SystemAPI.Query<GameObjectSource.New>()
			                                         .WithNone<GameObjectRef>()
			                                         .WithEntityAccess()) {
				ecb.AddComponent(entity, new GameObjectRef { Value = new GameObject() });
				ecb.AddComponent<ViewElement.Event.OnBorn>(entity);
				ecb.RemoveComponent<GameObjectSource.New>(entity);
			}
			
			ecb.Playback(state.EntityManager);
		}
	}

}
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Nakuru.Entities.Hybrid.Presentation
{

	[DisableAutoCreation]
	public partial struct GameObjectParentSystem : ISystem
	{
		[BurstCompile]
		public void OnCreate(ref SystemState state)
		{
			state.RequireForUpdate<ViewElement.Event.OnBorn>();
		}

		public void OnUpdate(ref SystemState state)
		{
			var parentTransformMap = new Dictionary<Entity, Transform>();

			foreach (var (goRef, entity) in SystemAPI.Query<GameObjectRef>()
			                                         .WithAll<ViewElement.Tag, Child>()
			                                         .WithEntityAccess()) {
				parentTransformMap.Add(entity, goRef.Value.transform);
			}
			
			if (parentTransformMap.Count <= 0)
				return;
			
			foreach (var (goRef, parentRo) in SystemAPI.Query<GameObjectRef, RefRO<Parent>>()
			                                         .WithAll<ViewElement.Tag, ViewElement.Event.OnBorn>()) {
				if (parentTransformMap.TryGetValue(parentRo.ValueRO.Value, out var parentTransform)) {
					goRef.Value.transform.SetParent(parentTransform);
				}
			}
		}
	}

}
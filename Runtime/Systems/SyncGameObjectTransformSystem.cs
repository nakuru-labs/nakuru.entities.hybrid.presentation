using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace MatchX.Client.Presentation
{
	
	[RequireMatchingQueriesForUpdate]
	public partial struct SyncGameObjectTransformSystem : ISystem
	{
		public void OnUpdate(ref SystemState state)
		{
			Debug.Log($"xxxx");
			foreach (var (viewRef, localToWorldRo, entity) in SystemAPI.Query<GameObjectRef, RefRO<LocalToWorld>>()
			                                             .WithAll<ViewElement.Tag, ViewElement.Event.OnTransformChanged>()
			                                             .WithEntityAccess()) {
				var entityName = state.EntityManager.GetName(entity);
				viewRef.Value.name = entityName;
				
				var goTransform = viewRef.Value.transform;
				var worldMatrix = localToWorldRo.ValueRO.Value;

				goTransform.position = worldMatrix.Translation();
				goTransform.rotation = worldMatrix.Rotation();
				goTransform.localScale = worldMatrix.Scale();
			}
		}
	}

}
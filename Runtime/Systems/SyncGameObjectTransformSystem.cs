using Unity.Entities;
using Unity.Transforms;

namespace Nakuru.Entities.Hybrid.Presentation
{
	
	[DisableAutoCreation]
	[RequireMatchingQueriesForUpdate]
	public partial struct SyncGameObjectTransformSystem : ISystem
	{
		public void OnUpdate(ref SystemState state)
		{
			foreach (var (viewRef, localTransformRo, postTransformMatrixRo, entity) in SystemAPI.Query<GameObjectRef, RefRO<LocalTransform>, RefRO<PostTransformMatrix>>()
			                                             .WithAll<ViewElement.Tag>()
			                                             .WithAny<ViewElement.Event.OnBorn, ViewElement.Event.OnTransformChanged>()
			                                             .WithEntityAccess()) {
				var entityName = state.EntityManager.GetName(entity);
				viewRef.Value.name = entityName;
				
				var goTransform = viewRef.Value.transform;
				var postTransformMatrix = postTransformMatrixRo.ValueRO.Value;
				var localTransform = localTransformRo.ValueRO;
				var scale = localTransform.Scale * postTransformMatrix.Scale();

				goTransform.localPosition = localTransform.Position;
				goTransform.localRotation = localTransform.Rotation;
				goTransform.localScale = scale;
			}
		}
	}

}
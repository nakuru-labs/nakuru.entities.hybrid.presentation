using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace MatchX.Client.Presentation
{

	[RequireMatchingQueriesForUpdate]
	public partial struct DetectTransformChangesSystem : ISystem
	{
		[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{
			var ecb = new EntityCommandBuffer(Allocator.Temp);
			
			foreach (var (previousMatrixRw, currentMatrixRo, entity) in SystemAPI.Query<RefRW<PreviousWorldMatrix>, RefRO<LocalToWorld>>()
			                                                         .WithAll<ViewElement.Tag>()
			                                                         .WithEntityAccess()) {
				if (previousMatrixRw.ValueRO.Value.Equals(currentMatrixRo.ValueRO.Value)) 
					continue;
				
				ecb.AddComponent<ViewElement.Event.OnTransformChanged>(entity);

				previousMatrixRw.ValueRW.Value = currentMatrixRo.ValueRO.Value;
			}
			
			ecb.Playback(state.EntityManager);
		}
	}

}
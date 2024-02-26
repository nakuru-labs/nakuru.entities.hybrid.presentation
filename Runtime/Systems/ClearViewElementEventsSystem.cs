using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace MatchX.Client.Presentation
{

	[RequireMatchingQueriesForUpdate]
	public partial struct ClearViewElementEventsSystem : ISystem
	{
		[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{
			var ecb = new EntityCommandBuffer(Allocator.Temp);
			
			foreach (var (_, entity) in SystemAPI.Query<ViewElement.Event.OnBorn>().WithEntityAccess()) {
				ecb.RemoveComponent<ViewElement.Event.OnBorn>(entity);
			}
			
			foreach (var (_, entity) in SystemAPI.Query<ViewElement.Event.OnTransformChanged>().WithEntityAccess()) {
				ecb.RemoveComponent<ViewElement.Event.OnTransformChanged>(entity);
			}
			
			ecb.Playback(state.EntityManager);
		}
	}

}
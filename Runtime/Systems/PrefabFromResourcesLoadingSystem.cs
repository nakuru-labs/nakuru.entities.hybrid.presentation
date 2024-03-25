using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Nakuru.Entities.Hybrid.Presentation
{

	[DisableAutoCreation]
	public partial struct PrefabFromResourcesLoadingSystem : ISystem
	{
		[BurstCompile]
		public void OnCreate(ref SystemState state) { }

		public void OnUpdate(ref SystemState state)
		{
			var ecb = new EntityCommandBuffer(Allocator.Temp);

			InitializeLoading(ref state, ecb);
			CheckLoadingCompletion(ref state, ecb);
			LoadPrefabSynchronously(ref state, ecb);

			ecb.Playback(state.EntityManager);
		}

		private void InitializeLoading(ref SystemState state, EntityCommandBuffer ecb)
		{
			foreach (var (prefabPath, entity) in SystemAPI.Query<RefRO<PrefabPath>>()
			                                              .WithAll<RequestPrefabFromResources, RequestPrefabAsync>()
			                                              .WithNone<PrefabRef, GameObjectRef, PrefabFromResourcesLoadingOperation>()
			                                              .WithEntityAccess()) {
				var path = prefabPath.ValueRO.Value.ToString();
				var loadingOperation = Resources.LoadAsync<GameObject>(path);

				ecb.AddComponent(entity, new PrefabFromResourcesLoadingOperation { Value = loadingOperation });
			}
		}

		private void CheckLoadingCompletion(ref SystemState state, EntityCommandBuffer ecb)
		{
			foreach (var (loadingOperation, entity) in SystemAPI.Query<PrefabFromResourcesLoadingOperation>()
			                                                    .WithAll<RequestPrefabFromResources>()
			                                                    .WithNone<PrefabRef, GameObjectRef>()
			                                                    .WithEntityAccess()) {
				if (!loadingOperation.Value.isDone)
					continue;

				ecb.AddComponent(entity, new PrefabRef { Value = (GameObject)loadingOperation.Value.asset });
				ecb.RemoveComponent<PrefabFromResourcesLoadingOperation>(entity);
				ecb.RemoveComponent<RequestPrefabAsync>(entity);
				ecb.RemoveComponent<RequestPrefabFromResources>(entity);
			}
		}

		private void LoadPrefabSynchronously(ref SystemState state, EntityCommandBuffer ecb)
		{
			foreach (var (prefabPath, entity) in SystemAPI.Query<RefRO<PrefabPath>>()
			                                              .WithAll<RequestPrefabFromResources>()
			                                              .WithNone<PrefabRef, GameObjectRef, RequestPrefabAsync>()
			                                              .WithEntityAccess()) {
				var path = prefabPath.ValueRO.Value.ToString();
				var prefab = Resources.Load<GameObject>(path);

				ecb.AddComponent(entity, new PrefabRef { Value = prefab });
			}
		}
	}

}
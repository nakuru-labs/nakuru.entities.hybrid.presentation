using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Nakuru.Entities.Hybrid.Presentation
{

	[DisableAutoCreation]
	public partial struct PrefabFromAddressablesLoadingSystem : ISystem
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
			                                              .WithAll<RequestPrefabFromAddressables, RequestPrefabAsync>()
			                                              .WithNone<PrefabRef, GameObjectRef, PrefabFromAddressablesLoadingOperation>()
			                                              .WithEntityAccess()) {
				var path = prefabPath.ValueRO.Value.ToString();
				var loadingOperation = Addressables.LoadAssetAsync<GameObject>(path);

				ecb.AddComponent(entity, new PrefabFromAddressablesLoadingOperation { Value = loadingOperation });
			}
		}

		private void CheckLoadingCompletion(ref SystemState state, EntityCommandBuffer ecb)
		{
			foreach (var (loadingOperation, entity) in SystemAPI.Query<PrefabFromAddressablesLoadingOperation>()
			                                                    .WithAll<RequestPrefabFromAddressables>()
			                                                    .WithNone<PrefabRef, GameObjectRef>()
			                                                    .WithEntityAccess()) {
				if (!loadingOperation.Value.IsDone)
					continue;

				ecb.AddComponent(entity, new PrefabRef { Value = loadingOperation.Value.Result });
				ecb.RemoveComponent<PrefabFromAddressablesLoadingOperation>(entity);
				ecb.RemoveComponent<RequestPrefabAsync>(entity);
				ecb.RemoveComponent<RequestPrefabFromAddressables>(entity);
			}
		}

		private void LoadPrefabSynchronously(ref SystemState state, EntityCommandBuffer ecb)
		{
			foreach (var (prefabPath, entity) in SystemAPI.Query<RefRO<PrefabPath>>()
			                                              .WithAll<RequestPrefabFromAddressables>()
			                                              .WithNone<PrefabRef, GameObjectRef, RequestPrefabAsync>()
			                                              .WithEntityAccess()) {
				var path = prefabPath.ValueRO.Value.ToString();
				var prefab = Addressables.LoadAssetAsync<GameObject>(path).WaitForCompletion();

				ecb.AddComponent(entity, new PrefabRef { Value = prefab });
			}
		}
	}

}
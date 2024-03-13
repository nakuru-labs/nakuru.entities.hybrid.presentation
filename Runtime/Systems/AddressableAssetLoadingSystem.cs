using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Nakuru.Entities.Hybrid.Presentation
{

	[DisableAutoCreation]
	[RequireMatchingQueriesForUpdate]
	public partial struct AddressableAssetLoadingSystem : ISystem
	{
		public void OnUpdate(ref SystemState state)
		{
			var ecb = new EntityCommandBuffer(Allocator.Temp);

			foreach (var (assetPathRo, entity) in SystemAPI.Query<RefRO<GameObjectSource.AssetPath>>()
			                                  .WithAll<ViewElement.Tag>()
			                                  .WithNone<GameObjectRef>()
			                                  .WithEntityAccess()) {

				var assetKey = assetPathRo.ValueRO.Value.ToString();
				var prefab = Addressables.LoadAssetAsync<GameObject>(assetKey).WaitForCompletion();
				Addressables.InstantiateAsync()
				var instance = Object.Instantiate(prefab);
				var ss = Resources.LoadAsync("");
				ss.asset
				ecb.AddComponent(entity, new GameObjectRef { Value = instance });
				ecb.AddComponent<ViewElement.Event.OnBorn>(entity);
			}
			
			ecb.Playback(state.EntityManager);
		}
	}

}
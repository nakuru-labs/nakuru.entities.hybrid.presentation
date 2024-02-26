using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace MatchX.Client
{

	[RequireMatchingQueriesForUpdate]
	public partial struct AddressableAssetLoadingSystem : ISystem
	{
		public void OnUpdate(ref SystemState state)
		{
			var ecb = new EntityCommandBuffer(Allocator.Temp);

			foreach (var (assetPathRo, entity) in SystemAPI.Query<RefRO<AddressableAssetPath>>()
			                                  .WithAll<ViewElement.Tag>()
			                                  .WithNone<GameObjectRef>()
			                                  .WithEntityAccess()) {

				var assetKey = assetPathRo.ValueRO.Value.ToString();
				var prefab = Addressables.LoadAssetAsync<GameObject>(assetKey).WaitForCompletion();
				var instance = Object.Instantiate(prefab);
				
				ecb.AddComponent(entity, new GameObjectRef { Value = instance });
				ecb.AddComponent<ViewElement.Event.OnBorn>(entity);
			}
			
			ecb.Playback(state.EntityManager);
		}
	}

}
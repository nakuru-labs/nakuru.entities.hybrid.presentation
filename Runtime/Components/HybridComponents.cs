using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Nakuru.Entities.Hybrid.Presentation
{
	
	public partial struct ViewElement
	{
		public struct Tag : IComponentData
		{ }
	}
	
	public struct PreviousWorldMatrix : IComponentData
	{
		public float4x4 Value;
	}
	
	public struct PrefabOriginResources : ICleanupComponentData
	{ }
	
	public struct PrefabOriginAddressables : ICleanupComponentData
	{ }
	
	public struct RequestNewGameObject : IComponentData
	{ }
	
	public struct PrefabPath : IComponentData
	{
		public FixedString128Bytes Value;
	}

	public struct RequestPrefabAsync : IComponentData
	{ }
	
	public class PrefabFromResourcesLoadingOperation : IComponentData
	{
		public ResourceRequest Value;
	}
	
	public class PrefabFromAddressablesLoadingOperation : IComponentData
	{
		public AsyncOperationHandle<GameObject> Value;
	}
	
	public class PrefabRef : ICleanupComponentData
	{
		public GameObject Value;
	}
	
	public class GameObjectRef : ICleanupComponentData
	{
		public GameObject Value;
	}

}
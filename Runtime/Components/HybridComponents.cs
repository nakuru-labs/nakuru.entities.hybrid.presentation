using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

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
	
	public class GameObjectRef : IComponentData
	{
		public GameObject Value;
	}

	public struct GameObjectSource
	{
		public struct New : IComponentData
		{ }

		public struct AssetPath : IComponentData
		{
			public FixedString128Bytes Value;
		}
		
		public struct AssetPathAsync : IComponentData
		{
			public FixedString128Bytes Value;
		}
	}

}
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace MatchX.Client
{
	public struct PreviousWorldMatrix : IComponentData
	{
		public float4x4 Value;
	}
	
	public struct AddressableAssetPath : IComponentData
	{
		public FixedString128Bytes Value;
	}
	
	public struct CreateGameObject : IComponentData
	{ }
	
	public struct ViewElement
	{
		public struct Tag : IComponentData
		{ }

		public struct Event
		{
			public struct OnBorn : IComponentData
			{ }
			
			public struct OnTransformChanged : IComponentData, IEnableableComponent
			{ }
		}

		public struct Factory : IComponentData
		{
			public Configurator.Instant Create(EntityManager entityManager)
			{
				var entity = entityManager.CreateEntity();
				return new Configurator.Instant(entity, entityManager)
				       .WithComponent<Tag>()
				       .WithComponent(LocalTransform.Identity)
				       .WithComponent<PreviousWorldMatrix>()
				       .WithComponent<LocalToWorld>()
				       .WithComponent(new PostTransformMatrix { Value = float4x4.identity });
				
			}
			
			public Configurator.Delayed Create(EntityCommandBuffer ecb)
			{
				var entity = ecb.CreateEntity();

				return new Configurator.Delayed(entity, ecb)
				       .WithComponent<Tag>()
				       .WithComponent(LocalTransform.Identity)
				       .WithComponent<PreviousWorldMatrix>()
				       .WithComponent<LocalToWorld>()
				       .WithComponent(new PostTransformMatrix { Value = float4x4.identity });
			}
		}
		
		public struct Configurator
		{
			public struct Instant
			{
				private Entity _entity;
				private EntityManager _entityManager;

				internal Instant(Entity entity, EntityManager entityManager)
				{
					_entity = entity;
					_entityManager = entityManager;
				}

				public Instant WithName(FixedString64Bytes name)
				{
					_entityManager.SetName(_entity, name);
					return this;
				}

				public Instant WithComponent<T>() where T : unmanaged, IComponentData
				{
					_entityManager.AddComponent<T>(_entity);
					return this;
				}

				public Instant WithComponent<T>(T component) where T : unmanaged, IComponentData
				{
					_entityManager.AddComponentData(_entity, component);
					return this;
				}
				
				public Instant WithManagedComponent<T>() where T : class, IComponentData
				{
					_entityManager.AddComponent<T>(_entity);
					return this;
				}
				
				public Instant WithManagedComponent<T>(T component) where T : class, IComponentData
				{
					_entityManager.AddComponentObject(_entity, component);
					return this;
				}
			}
			
			public struct Delayed
			{
				public Entity Entity { get; private set; }
				
				private EntityCommandBuffer _ecb;
				private LocalTransform _localTransform;
				private PostTransformMatrix _postTransformMatrix;

				internal Delayed(Entity entity, EntityCommandBuffer ecb)
				{
					Entity = entity;
					_ecb = ecb;
					_localTransform = LocalTransform.Identity;
					_postTransformMatrix = new PostTransformMatrix { Value = float4x4.identity };
				}
				
				public Delayed WithName(FixedString64Bytes name)
				{
					_ecb.SetName(Entity, name);
					return this;
				}

				public Delayed WithComponent<T>() where T : unmanaged, IComponentData
				{
					_ecb.AddComponent<T>(Entity);
					return this;
				}

				public Delayed WithComponent<T>(T component) where T : unmanaged, IComponentData
				{
					_ecb.AddComponent(Entity, component);
					return this;
				}
				
				public Delayed WithManagedComponent<T>() where T : class, IComponentData
				{
					_ecb.AddComponent<T>(Entity);
					return this;
				}
				
				public Delayed WithManagedComponent<T>(T component) where T : class, IComponentData
				{
					_ecb.AddComponent(Entity, component);
					return this;
				}
				
				public Delayed WithEnableableComponent<T>(bool enabled = true) where T : unmanaged, IComponentData, IEnableableComponent
				{
					_ecb.AddComponent<T>(Entity);
					_ecb.SetComponentEnabled<T>(Entity, enabled);
					return this;
				}

				public Delayed WithPosition(float3 value)
				{
					_localTransform.Position = value;
					return WithComponent(_localTransform);
				}
				
				public Delayed WithRotation(quaternion value)
				{
					_localTransform.Rotation = value;
					return WithComponent(_localTransform);
				}
				
				public Delayed WithScale(float value)
				{
					_localTransform.Scale = value;
					return WithComponent(_localTransform);
				}
				
				public Delayed WithScale(float3 value)
				{
					_postTransformMatrix.Value = float4x4.Scale(value);
					return WithComponent(_postTransformMatrix);
				}
				
				public Delayed WithAddressableAssetPath(FixedString128Bytes value) =>
					WithComponent(new AddressableAssetPath { Value = value });
				
				public Delayed WithNewGameObject() => WithComponent<CreateGameObject>();
				
				public Delayed WithParent(Entity parentEntity) => WithComponent(new Parent { Value = parentEntity });
			}
		}
	}
	
	public class GameObjectRef : IComponentData
	{
		public GameObject Value;
	}

}
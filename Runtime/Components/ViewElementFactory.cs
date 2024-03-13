using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Nakuru.Entities.Hybrid.Presentation
{

	public partial struct ViewElement
	{
		public struct Factory : IComponentData
		{
			public Configurator Create(EntityManager entityManager)
			{
				var entity = entityManager.CreateEntity();
				return new Configurator(entity, entityManager)
				       .WithComponent<Tag>()
				       .WithComponent(LocalTransform.Identity)
				       .WithComponent<PreviousWorldMatrix>()
				       .WithComponent<LocalToWorld>()
				       .WithComponent(new PostTransformMatrix { Value = float4x4.identity })
				       .WithBuffer<LinkedEntityGroup>()
				       .WithBufferElement<LinkedEntityGroup>(entity);
			}
			
			public Configurator Create(EntityCommandBuffer ecb)
			{
				var entity = ecb.CreateEntity();
				return new Configurator(entity, ecb)
				       .WithComponent<Tag>()
				       .WithComponent(LocalTransform.Identity)
				       .WithComponent<PreviousWorldMatrix>()
				       .WithComponent<LocalToWorld>()
				       .WithComponent(new PostTransformMatrix { Value = float4x4.identity })
				       .WithBuffer<LinkedEntityGroup>()
				       .WithBufferElement<LinkedEntityGroup>(entity);
			}
		}
		
		public struct Configurator
		{
			public Entity Entity { get; private set; }
				
			internal EntityCommandBuffer Ecb { get; private set; }
			internal EntityManager EntityManager { get; private set; }
			internal LocalTransform LocalTransform;
			internal PostTransformMatrix PostTransformMatrix;

			internal Configurator(Entity entity, EntityCommandBuffer ecb)
			{
				Entity = entity;
				Ecb = ecb;
				EntityManager = default;
				
				LocalTransform = LocalTransform.Identity;
				PostTransformMatrix = new PostTransformMatrix { Value = float4x4.identity };
			}
			
			internal Configurator(Entity entity, EntityManager entityManager)
			{
				Entity = entity;
				EntityManager = entityManager;
				Ecb = default;
				
				LocalTransform = LocalTransform.Identity;
				PostTransformMatrix = new PostTransformMatrix { Value = float4x4.identity };
			}
		}
	}

}
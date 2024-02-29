using NUnit.Framework;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Nakuru.Entities.Hybrid.Presentation.Tests
{
	
	public class EntityConfigurator_UsingCommandBuffer_Tests : EcsTestFixture
	{
		private EntityCommandBuffer GetEcb()
		{
			var ecbSystem = World.GetOrCreateSystemManaged<BeginSimulationEntityCommandBufferSystem>();
			return ecbSystem.CreateCommandBuffer();
		}

		[Test]
		public void Create()
		{
			var query = new EntityQueryBuilder(Allocator.Temp)
			            .WithAll<ViewElement.Tag>()
			            .Build(EntityManager);

			Assert.That(query.CalculateEntityCount(), Is.EqualTo(0));

			ViewFactory.Create(GetEcb());

			Update();

			Assert.That(query.CalculateEntityCount(), Is.EqualTo(1));
		}

		[Test]
		public void WithName()
		{
			var initialEntityName = "TestEntityName";

			var query = new EntityQueryBuilder(Allocator.Temp)
			            .WithAll<ViewElement.Tag>()
			            .Build(EntityManager);

			ViewFactory.Create(GetEcb())
			           .WithName(initialEntityName);

			Update();

			var entity = query.GetSingletonEntity();
			var actualEntityName = EntityManager.GetName(entity);

			Assert.That(actualEntityName, Is.EqualTo(initialEntityName));
		}

		[Test]
		public void WithComponentGeneric()
		{
			var query = new EntityQueryBuilder(Allocator.Temp)
			            .WithAll<ViewElement.Tag>()
			            .Build(EntityManager);

			ViewFactory.Create(GetEcb())
			           .WithComponent<TestComponents.UnmanagedTag>();

			Update();

			var entity = query.GetSingletonEntity();

			Assert.IsTrue(EntityManager.HasComponent<TestComponents.UnmanagedTag>(entity));
		}

		[Test]
		public void WithManagedComponentGeneric()
		{
			var query = new EntityQueryBuilder(Allocator.Temp)
			            .WithAll<ViewElement.Tag>()
			            .Build(EntityManager);

			ViewFactory.Create(GetEcb())
			           .WithManagedComponent<TestComponents.ManagedTag>();

			Update();

			var entity = query.GetSingletonEntity();

			Assert.IsTrue(EntityManager.HasComponent<TestComponents.ManagedTag>(entity));
		}

		[Test]
		public void WithComponentData()
		{
			var query = new EntityQueryBuilder(Allocator.Temp)
			            .WithAll<ViewElement.Tag>()
			            .Build(EntityManager);

			ViewFactory.Create(GetEcb())
			           .WithComponent(new TestComponents.UnmanagedTag());

			Update();

			var entity = query.GetSingletonEntity();

			Assert.IsTrue(EntityManager.HasComponent<TestComponents.UnmanagedTag>(entity));
		}

		[Test]
		public void WithManagedComponentData()
		{
			var query = new EntityQueryBuilder(Allocator.Temp)
			            .WithAll<ViewElement.Tag>()
			            .Build(EntityManager);

			ViewFactory.Create(GetEcb())
			           .WithManagedComponent(new TestComponents.ManagedTag());

			Update();

			var entity = query.GetSingletonEntity();

			Assert.IsTrue(EntityManager.HasComponent<TestComponents.ManagedTag>(entity));
		}

		[Test]
		public void WithPosition()
		{
			var initialPosition = new float3(1f, 2f, 3f);

			var query = new EntityQueryBuilder(Allocator.Temp)
			            .WithAll<ViewElement.Tag, LocalTransform>()
			            .Build(EntityManager);

			ViewFactory.Create(GetEcb())
			           .WithPosition(initialPosition);

			Update();

			var localTransform = query.GetSingleton<LocalTransform>();

			Assert.That(localTransform.Position, Is.EqualTo(initialPosition));
		}

		[Test]
		public void WithRotation()
		{
			var initialRotation = new quaternion(10, 15, 44, 1);

			var query = new EntityQueryBuilder(Allocator.Temp)
			            .WithAll<ViewElement.Tag, LocalTransform>()
			            .Build(EntityManager);

			ViewFactory.Create(GetEcb())
			           .WithRotation(initialRotation);

			Update();

			var localTransform = query.GetSingleton<LocalTransform>();

			Assert.That(localTransform.Rotation, Is.EqualTo(initialRotation));
		}

		[Test]
		public void WithUniformScale()
		{
			var initialScale = 3f;

			var query = new EntityQueryBuilder(Allocator.Temp)
			            .WithAll<ViewElement.Tag, LocalTransform>()
			            .Build(EntityManager);

			ViewFactory.Create(GetEcb())
			           .WithScale(initialScale);

			Update();

			var localTransform = query.GetSingleton<LocalTransform>();

			Assert.That(localTransform.Scale, Is.EqualTo(initialScale));
		}

		[Test]
		public void WithNonUniformScale()
		{
			var initialScale = new float3(2, 1, 12);

			var query = new EntityQueryBuilder(Allocator.Temp)
			            .WithAll<ViewElement.Tag, PostTransformMatrix>()
			            .Build(EntityManager);

			ViewFactory.Create(GetEcb())
			           .WithScale(initialScale);

			Update();

			var postTransformMatrix = query.GetSingleton<PostTransformMatrix>();

			Assert.That(postTransformMatrix.Value.Scale(), Is.EqualTo(initialScale));
		}

		[Test]
		public void WithParent()
		{
			var ecb = GetEcb();
			var tag1Query = new EntityQueryBuilder(Allocator.Temp)
			                .WithAll<ViewElement.Tag, TestComponents.Tag1>()
			                .Build(EntityManager);

			var tag2Query = new EntityQueryBuilder(Allocator.Temp)
			                .WithAll<ViewElement.Tag, TestComponents.Tag2>()
			                .Build(EntityManager);

			var entity1Builder = ViewFactory.Create(ecb)
			                                .WithComponent<TestComponents.Tag1>();

			ViewFactory.Create(ecb)
			           .WithComponent<TestComponents.Tag2>()
			           .WithParent(entity1Builder.Entity);

			Update();

			var tag1Entity = tag1Query.GetSingletonEntity();
			var tag2Entity = tag2Query.GetSingletonEntity();

			Assert.That(tag1Entity, Is.Not.EqualTo(Entity.Null));
			Assert.That(tag2Entity, Is.Not.EqualTo(Entity.Null));

			var parent = EntityManager.GetComponentData<Parent>(tag2Entity);

			Assert.That(parent.Value, Is.EqualTo(tag1Entity));
		}
	}

}
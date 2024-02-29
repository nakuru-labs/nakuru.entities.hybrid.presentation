using NUnit.Framework;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Nakuru.Entities.Hybrid.Presentation.Tests
{

	public class EntityConfigurator_UsingEntityManager_Tests : EcsTestFixture
	{
		[Test]
		public void Create()
		{
			var query = new EntityQueryBuilder(Allocator.Temp)
			            .WithAll<ViewElement.Tag>()
			            .Build(EntityManager);
			
			Assert.That(query.CalculateEntityCount(), Is.EqualTo(0));

			ViewFactory.Create(EntityManager);

			Assert.That(query.CalculateEntityCount(), Is.EqualTo(1));
		}
		
		[Test]
		public void WithName()
		{
			var initialEntityName = "TestEntityName";
			var query = new EntityQueryBuilder(Allocator.Temp)
			            .WithAll<ViewElement.Tag>()
			            .Build(EntityManager);

			ViewFactory.Create(EntityManager)
			           .WithName(initialEntityName);

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

			ViewFactory.Create(EntityManager)
			           .WithComponent<TestComponents.UnmanagedTag>();

			var entity = query.GetSingletonEntity();
			
			Assert.IsTrue(EntityManager.HasComponent<TestComponents.UnmanagedTag>(entity));
		}
		
		[Test]
		public void WithManagedComponentGeneric()
		{
			var query = new EntityQueryBuilder(Allocator.Temp)
			            .WithAll<ViewElement.Tag>()
			            .Build(EntityManager);

			ViewFactory.Create(EntityManager)
			           .WithManagedComponent<TestComponents.ManagedTag>();

			var entity = query.GetSingletonEntity();
			
			Assert.IsTrue(EntityManager.HasComponent<TestComponents.ManagedTag>(entity));
		}
		
		[Test]
		public void WithComponentData()
		{
			var query = new EntityQueryBuilder(Allocator.Temp)
			            .WithAll<ViewElement.Tag>()
			            .Build(EntityManager);

			ViewFactory.Create(EntityManager)
			           .WithComponent(new TestComponents.UnmanagedTag());

			var entity = query.GetSingletonEntity();
			
			Assert.IsTrue(EntityManager.HasComponent<TestComponents.UnmanagedTag>(entity));
		}
		
		[Test]
		public void WithManagedComponentData()
		{
			var query = new EntityQueryBuilder(Allocator.Temp)
			            .WithAll<ViewElement.Tag>()
			            .Build(EntityManager);

			ViewFactory.Create(EntityManager)
			           .WithManagedComponent(new TestComponents.ManagedTag());

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

			ViewFactory.Create(EntityManager)
			           .WithPosition(initialPosition);

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

			ViewFactory.Create(EntityManager)
			           .WithRotation(initialRotation);
			
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

			ViewFactory.Create(EntityManager)
			           .WithScale(initialScale);

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

			ViewFactory.Create(EntityManager)
			           .WithScale(initialScale);

			var postTransformMatrix = query.GetSingleton<PostTransformMatrix>();
			
			Assert.That(postTransformMatrix.Value.Scale(), Is.EqualTo(initialScale));
		}
		
		[Test]
		public void WithParent()
		{
			var tag1Query = new EntityQueryBuilder(Allocator.Temp)
			                .WithAll<ViewElement.Tag, TestComponents.Tag1>()
			                .Build(EntityManager);
			
			var tag2Query = new EntityQueryBuilder(Allocator.Temp)
			                .WithAll<ViewElement.Tag, TestComponents.Tag2>()
			                .Build(EntityManager);
			
			var entity1Builder = ViewFactory.Create(EntityManager)
			                                .WithComponent<TestComponents.Tag1>();

			ViewFactory.Create(EntityManager)
			           .WithComponent<TestComponents.Tag2>()
			           .WithParent(entity1Builder.Entity);

			var tag1Entity = tag1Query.GetSingletonEntity();
			var tag2Entity = tag2Query.GetSingletonEntity();
			
			Assert.That(tag1Entity, Is.Not.EqualTo(Entity.Null));
			Assert.That(tag2Entity, Is.Not.EqualTo(Entity.Null));
			
			var parent = EntityManager.GetComponentData<Parent>(tag2Entity);
			
			Debug.Log($"tag1Entity - {tag1Entity}");
			Debug.Log($"tag2Entity - {tag2Entity}");

			Debug.Log($"tag1Entity.HasTag1)- {EntityManager.HasComponent<TestComponents.Tag1>(tag1Entity)}");
			Debug.Log($"tag1Entity.HasTag2)- {EntityManager.HasComponent<TestComponents.Tag2>(tag1Entity)}");

			Debug.Log($"tag2Entity.HasTag1)- {EntityManager.HasComponent<TestComponents.Tag1>(tag2Entity)}");
			Debug.Log($"tag2Entity.HasTag2)- {EntityManager.HasComponent<TestComponents.Tag2>(tag2Entity)}");
			
			Assert.That(parent.Value, Is.EqualTo(tag1Entity));
		}
	}

}
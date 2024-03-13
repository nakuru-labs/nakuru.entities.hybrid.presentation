using NUnit.Framework;
using Unity.Collections;
using Unity.Entities;

namespace Nakuru.Entities.Hybrid.Presentation.Tests
{

	public class InfrastructureTests : EcsTestFixture
	{
		[Test]
		public void When_DestroyRootEntity_AllChildrenWillBeDestroyedAlso()
		{
			var query = new EntityQueryBuilder(Allocator.Temp)
			            .WithAll<ViewElement.Tag>()
			            .Build(EntityManager);

			// parent entity
			var parentEntity = ViewFactory.Create(EntityManager).Entity;
			
			// first level child entity
			var childEntity1 = ViewFactory.Create(EntityManager)
			                              .WithParent(parentEntity)
			                              .Entity;

			// second level child entity
			ViewFactory.Create(EntityManager)
			           .WithParent(childEntity1);
			
			Assert.That(query.CalculateEntityCount(), Is.EqualTo(3));

			EntityManager.DestroyEntity(parentEntity);

			Assert.That(query.CalculateEntityCount(), Is.EqualTo(0));
		}
	}

}
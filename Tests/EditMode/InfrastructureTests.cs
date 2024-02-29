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

			var parentEntity = ViewFactory.Create(EntityManager).Entity;

			var child1Entity = ViewFactory.Create(EntityManager)
			                              .WithParent(parentEntity)
			                              .Entity;

			ViewFactory.Create(EntityManager)
			           .WithParent(child1Entity);
			
			Assert.That(query.CalculateEntityCount(), Is.EqualTo(3));

			EntityManager.DestroyEntity(parentEntity);

			Assert.That(query.CalculateEntityCount(), Is.EqualTo(0));
		}
	}

}
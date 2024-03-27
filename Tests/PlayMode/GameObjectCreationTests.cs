using System.Collections;
using Nakuru.Entities.Hybrid.Presentation;
using Nakuru.Entities.Hybrid.Presentation.Tests;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.PlayMode
{

	public class GameObjectCreationTests : EcsTestFixture
	{
		[UnityTest]
		public IEnumerator CreateNewGameObject()
		{
			var goName = "TestGameObject";
			
			Assert.That(GameObject.Find(goName), Is.Null);

			var entity = ViewFactory.Create(EntityManager)
			           .WithName(goName)
			           .WithGameObjectNew()
			           .Entity;

			Update();
			yield return null;
			
			Assert.That(GameObject.Find(goName), Is.Not.Null);

			EntityManager.DestroyEntity(entity);
			Update();
			
			yield return null;
		}
		
		[Test]
		public void LoadGameObjectFromResourcesSynchronously()
		{
			var resourcesPrefabName = "TestPrefab";
			var goName = "TestGameObject";
			
			Assert.That(GameObject.Find(goName), Is.Null);

			var entity = ViewFactory.Create(EntityManager)
			           .WithName(goName)
			           .WithGameObjectFromResources(resourcesPrefabName, false)
			           .Entity;

			Update();
			
			Assert.That(GameObject.Find(goName), Is.Not.Null);
			
			EntityManager.DestroyEntity(entity);
			Update();
		}
		
		[UnityTest]
		public IEnumerator LoadGameObjectFromResourcesAsynchronously()
		{
			var resourcesPrefabName = "TestPrefab";
			var goName = "TestGameObject";
			
			Assert.That(GameObject.Find(goName), Is.Null);

			var entity = ViewFactory.Create(EntityManager)
			           .WithName(goName)
			           .WithGameObjectFromResources(resourcesPrefabName)
			           .Entity;

			yield return new WaitUntil(() => {
				Update();
				return EntityManager.HasComponent<GameObjectRef>(entity);
			});
			
			yield return null;
			
			Assert.That(GameObject.Find(goName), Is.Not.Null);
			
			EntityManager.DestroyEntity(entity);
			Update();
			
			yield return null;
		}
	}

}
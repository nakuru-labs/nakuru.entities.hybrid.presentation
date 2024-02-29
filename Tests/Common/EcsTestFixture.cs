using NUnit.Framework;
using Unity.Entities;

namespace Nakuru.Entities.Hybrid.Presentation.Tests
{

	[TestFixture]
	public abstract class EcsTestFixture
	{
		protected World World { get; private set; }
		protected EntityManager EntityManager { get; private set; }

		private World _previousWorld;

		protected ViewElement.Factory ViewFactory;

		[SetUp]
		public virtual void Setup()
		{
			_previousWorld = World.DefaultGameObjectInjectionWorld;
			World = new World("Test World", WorldFlags.Game);
			World.DefaultGameObjectInjectionWorld = World;
			EntityManager = World.EntityManager;
			ViewFactory = new();

			var systems = DefaultWorldInitialization.GetAllSystems(WorldSystemFilterFlags.Default);
			DefaultWorldInitialization.AddSystemsToRootLevelSystemGroups(World, systems);
			
			Update();
		}

		protected void Update()
		{
			if (World is not { IsCreated: true })
				return;
			
			World.Update();
		}

		[TearDown]
		public virtual void TearDown()
		{
			if (World is not { IsCreated: true })
				return;
			
			// Note that World.Dispose() already completes all jobs. But some tests may leave tests running when
			// they return, but we can't safely run an internal consistency check with jobs running, so we
			// explicitly complete them here as well.
			World.EntityManager.CompleteAllTrackedJobs();
			
			World.DestroyAllSystemsAndLogException();

			World.Dispose();
			World = null;

			World.DefaultGameObjectInjectionWorld = _previousWorld;
			
			_previousWorld = null;
			EntityManager = default;
		}
		
		protected struct TestComponents
		{
			public struct Tag1 : IComponentData
			{ }
			
			public struct Tag2 : IComponentData
			{ }
			
			public struct UnmanagedTag : IComponentData
			{ }
		
			public class ManagedTag : IComponentData
			{ }
		}
	}

}
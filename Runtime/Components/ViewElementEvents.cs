using Unity.Entities;

namespace Nakuru.Entities.Hybrid.Presentation
{

	public partial struct ViewElement
	{
		public struct Event
		{
			public struct OnBorn : IComponentData { }
			public struct OnTransformChanged : IComponentData, IEnableableComponent { }
		}
	}

}
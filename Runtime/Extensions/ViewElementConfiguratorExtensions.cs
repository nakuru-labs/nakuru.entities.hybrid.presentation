using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Nakuru.Entities.Hybrid.Presentation
{

	public static class ViewElementConfiguratorExtensions
	{
		public static ViewElement.Configurator WithName(this ViewElement.Configurator cfg, FixedString64Bytes name)
		{
			if (cfg.Ecb.IsCreated) {
				cfg.Ecb.SetName(cfg.Entity, name);
			} else {
				cfg.EntityManager.SetName(cfg.Entity, name);
			}
			
			return cfg;
		}
		
		public static ViewElement.Configurator WithComponent<T>(this ViewElement.Configurator cfg) where T : unmanaged, IComponentData
		{
			if (cfg.Ecb.IsCreated) {
				cfg.Ecb.AddComponent<T>(cfg.Entity);
			} else {
				cfg.EntityManager.AddComponent<T>(cfg.Entity);
			}
			
			return cfg;
		}

		public static ViewElement.Configurator WithComponent<T>(this ViewElement.Configurator cfg, T component) where T : unmanaged, IComponentData
		{
			if (cfg.Ecb.IsCreated) {
				cfg.Ecb.AddComponent(cfg.Entity, component);
			} else {
				cfg.EntityManager.AddComponentData(cfg.Entity, component);
			}
			
			return cfg;
		}
			
		public static ViewElement.Configurator WithManagedComponent<T>(this ViewElement.Configurator cfg) where T : class, IComponentData
		{
			if (cfg.Ecb.IsCreated) {
				cfg.Ecb.AddComponent<T>(cfg.Entity);
			} else {
				cfg.EntityManager.AddComponent<T>(cfg.Entity);
			}
			
			return cfg;
		}
			
		public static ViewElement.Configurator WithManagedComponent<T>(this ViewElement.Configurator cfg, T component) where T : class, IComponentData
		{
			if (cfg.Ecb.IsCreated) {
				cfg.Ecb.AddComponent(cfg.Entity, component);
			} else {
				cfg.EntityManager.AddComponentObject(cfg.Entity, component);
			}
			
			return cfg;
		}
		
		public static ViewElement.Configurator WithBuffer<T>(this ViewElement.Configurator cfg) where T : unmanaged, IBufferElementData
		{
			if (cfg.Ecb.IsCreated) {
				cfg.Ecb.AddBuffer<T>(cfg.Entity);
			} else {
				cfg.EntityManager.AddBuffer<T>(cfg.Entity);
			}
			
			return cfg;
		}
		
		public static ViewElement.Configurator WithBufferElement<T>(this ViewElement.Configurator cfg, T bufferElement) where T : unmanaged, IBufferElementData
		{
			if (cfg.Ecb.IsCreated) {
				cfg.Ecb.AppendToBuffer(cfg.Entity, bufferElement);
			} else {
				cfg.EntityManager.GetBuffer<T>(cfg.Entity).Add(bufferElement);
			}
			
			return cfg;
		}
		
		public static ViewElement.Configurator WithBufferElements<T>(this ViewElement.Configurator cfg, NativeArray<T> bufferElements) where T : unmanaged, IBufferElementData
		{
			if (cfg.Ecb.IsCreated) {
				foreach (var bufferElement in bufferElements) {
					cfg.Ecb.AppendToBuffer(cfg.Entity, bufferElement);
				}
			} else {
				cfg.EntityManager.GetBuffer<T>(cfg.Entity).AddRange(bufferElements);
			}
			
			return cfg;
		}
			
		public static ViewElement.Configurator WithEnableableComponent<T>(this ViewElement.Configurator cfg, bool enabled = true) where T : unmanaged, IComponentData, IEnableableComponent
		{
			if (cfg.Ecb.IsCreated) {
				cfg.Ecb.AddComponent<T>(cfg.Entity);
				cfg.Ecb.SetComponentEnabled<T>(cfg.Entity, enabled);
			} else {
				cfg.EntityManager.AddComponent<T>(cfg.Entity);
				cfg.EntityManager.SetComponentEnabled<T>(cfg.Entity, enabled);
			}
			
			return cfg;
		}
		
		public static ViewElement.Configurator WithPosition(this ViewElement.Configurator cfg, float3 value)
		{
			cfg.LocalTransform.Position = value;
			return cfg.WithComponent(cfg.LocalTransform);
		}
			
		public static ViewElement.Configurator WithRotation(this ViewElement.Configurator cfg, quaternion value)
		{
			cfg.LocalTransform.Rotation = value;
			return cfg.WithComponent(cfg.LocalTransform);
		}
			
		public static ViewElement.Configurator WithScale(this ViewElement.Configurator cfg, float value)
		{
			cfg.LocalTransform.Scale = value;
			return cfg.WithComponent(cfg.LocalTransform);
		}
			
		public static ViewElement.Configurator WithScale(this ViewElement.Configurator cfg, float3 value)
		{
			cfg.PostTransformMatrix.Value = float4x4.Scale(value);
			return cfg.WithComponent(cfg.PostTransformMatrix);
		}

		public static ViewElement.Configurator WithParent(this ViewElement.Configurator cfg, Entity parentEntity)
		{
			cfg.WithComponent(new Parent { Value = parentEntity });
			
			if (cfg.Ecb.IsCreated) {
				cfg.Ecb.AppendToBuffer<LinkedEntityGroup>(parentEntity, cfg.Entity);
			} else {
				cfg.EntityManager.GetBuffer<LinkedEntityGroup>(parentEntity).Add(cfg.Entity);
			}

			return cfg;
		}
			
		public static ViewElement.Configurator WithGameObjectNew(this ViewElement.Configurator cfg) => 
			cfg.WithComponent<RequestNewGameObject>();

		public static ViewElement.Configurator WithGameObjectFromResources(
			this ViewElement.Configurator cfg, FixedString128Bytes path, bool async = true)
		{
			if (async)
				cfg.WithComponent<RequestPrefabAsync>();
			
			cfg.WithComponent<RequestPrefabFromResources>()
			   .WithComponent(new PrefabPath { Value = path });
			return cfg;
		}
		
		public static ViewElement.Configurator WithGameObjectFromAddressables(
			this ViewElement.Configurator cfg, FixedString128Bytes path, bool async = true)
		{
			if (async)
				cfg.WithComponent<RequestPrefabAsync>();
			
			cfg.WithComponent<RequestPrefabFromAddressables>()
			   .WithComponent(new PrefabPath { Value = path });
			return cfg;
		}
	}

}
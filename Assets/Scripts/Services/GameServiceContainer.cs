using System.Collections.Generic;
using System;
namespace Services
{
	public class GameServiceContainer : IServiceProvider
	{
		private Dictionary<Type, object> Container = new Dictionary<Type, object>();
		public void AddService(Type interfaceType, object service)
		{
			if (interfaceType == null || service == null)
			{
				Logger.Error("Type null or service null.");
				return;
			}
			if (Container.ContainsKey(interfaceType))
			{
				Logger.Error("Object with type " + interfaceType.ToString() + " existed in Container");
				return;
			}
			Container.Add(interfaceType, service);
		}
		public void AddService<T>(T provider) where T : class
		{
			if (provider == null)
			{
				Logger.Error("Service null.");
				return;
			}
			AddService(typeof(T), provider);
		}
		public object GetService(Type interfaceType)
		{
			if (interfaceType == null)
			{
				Logger.Error("Type null.");
				return null;
			}
			if (!Container.ContainsKey(interfaceType))
			{
				Logger.Error("Type " + interfaceType.ToString() + " doesn't exist in Container.");
				return null;
			}
			return Container[interfaceType];
		}
		public T GetService<T>() where T : class
		{
			if (typeof(T) == null)
			{
				Logger.Error("Type null.");
				return null;
			}
			if (!Container.ContainsKey(typeof(T)))
			{
				Logger.Error("Type " + typeof(T).ToString() + " doesn't exist in Container.");
				return null;
			}
			return (T)Container[typeof(T)];
		}
		public void RemoveService(Type interfaceType)
		{
			if (interfaceType == null)
			{
				Logger.Error("Type null.");
				return;
			}
			if (!Container.ContainsKey(interfaceType))
			{
				Logger.Error("Type " + interfaceType.ToString() + " doesn't exist in Container.");
				return;
			}
			Container.Remove(interfaceType);
		}
		public void RemoveService<T>() where T : class
		{
			if (typeof(T) == null)
			{
				Logger.Error("Type null.");
				return;
			}
			if (!Container.ContainsKey(typeof(T)))
			{
				Logger.Error("Type " + typeof(T).ToString() + " doesn't exist in Container.");
				return;
			}
			Container.Remove(typeof(T));
		}
	}
}

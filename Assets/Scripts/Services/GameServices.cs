using System;
using UnityEngine;

namespace Services
{
	public class GameServices : MonoBehaviour
	{
		public event Action OnDestroyAction;

		private GameServiceContainer gameServiceContainer = new GameServiceContainer();

		void Start()
		{
			DontDestroyOnLoad(gameObject);
		}

		private void OnDestroy()
		{
			OnDestroyAction?.Invoke();
		}

		public void AddService<T>(T provider) where T : class => gameServiceContainer.AddService(provider);
		public T GetService<T>() where T : class => gameServiceContainer.GetService<T>();
	}
}

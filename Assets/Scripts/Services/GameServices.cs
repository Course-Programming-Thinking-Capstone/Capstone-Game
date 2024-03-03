 using System;
using UnityEngine;
using Utility.Singleton;

namespace Services
{
    public class GameServices : PersistentMonoSingleton<GameServices>
    {

        private readonly GameServiceContainer gameServiceContainer = new GameServiceContainer();
        

        public void AddService<T>(T provider) where T : class => gameServiceContainer.AddService(provider);
        public T GetService<T>() where T : class => gameServiceContainer.GetService<T>();
    }
}
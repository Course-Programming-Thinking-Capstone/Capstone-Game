using System;
using UnityEngine;

namespace GameScene
{
    public class GameModel : MonoBehaviour
    {
        
    }
    
    [Serializable]
    public class ModelSelector
    {
        [SerializeField] private SelectType selectType;
        [SerializeField] private GameObject prefabs;

        public SelectType SelectType => selectType;

        public GameObject Prefabs => prefabs;
    }

    [Serializable]
    public class ModelSelected
    {
        [SerializeField] private SelectType selectType;
        [SerializeField] private GameObject prefabs;
        public SelectType SelectType => selectType;
        public GameObject Prefabs => prefabs;
    }
}
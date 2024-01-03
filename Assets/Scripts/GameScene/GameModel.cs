using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameScene
{
    public class GameModel : MonoBehaviour
    {
        [SerializeField] private List<ModelSelected> modelSelected;
        public GameObject GetSelected(SelectType selectType)
        {
            var result = modelSelected.FirstOrDefault(o => o.SelectType == selectType);
            return result?.Prefabs;
        }
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
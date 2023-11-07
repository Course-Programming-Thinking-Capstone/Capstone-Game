using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameScene
{
    public class GameModel : MonoBehaviour
    {
        [SerializeField] private List<ModelSelector> modelSelector;
        [SerializeField] private List<ModelSelected> modelSelected;
        [SerializeField] private List<Sprite> candySprites;
        [SerializeField] private GameObject playerModel;
        [SerializeField] private GameObject candyModel;
        [SerializeField] private float playerMoveTime;

        public GameObject PlayerModel => playerModel;
        public float PlayerMoveTime => playerMoveTime;
        public List<Sprite> CandySprites => candySprites;
        public GameObject CandyModel => candyModel;

        public GameObject GetSelector(SelectType selectType)
        {
            var result = modelSelector.FirstOrDefault(o => o.SelectType == selectType);
            return result?.Prefabs;
        }
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
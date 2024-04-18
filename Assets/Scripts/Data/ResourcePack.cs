using System.Collections.Generic;
using GameScene;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "ControlItem", menuName = "GamePlay/GameControlItemGroupData", order = -999)]
    public class ResourcePack : ScriptableObject
    {
        [SerializeField] private List<GameControlItemData> gameControlItemData;
        [SerializeField] private GameObject playerModel;
        [SerializeField] private GameObject targetModel;

        [Header("Basic mode")]
        [SerializeField] private GameObject boardRoadCell;
        [SerializeField] private GameObject roadSelector;
        [Header("Control mode")]
        [SerializeField] private GameObject selectorModel;
        [SerializeField] private GameObject selectedModel;
        [SerializeField] private GameObject boardCellModel;
        [SerializeField] private GameObject loopPrefab;
        [SerializeField] private GameObject conditionPrefab;
        
        public List<GameControlItemData> GameControlItemData => gameControlItemData;
        public GameObject ConditionPrefab => conditionPrefab;
        public GameObject BoardRoadCell => boardRoadCell;
        public GameObject RoadSelector => roadSelector;
        public GameObject PlayerModel => playerModel;
        public GameObject TargetModel => targetModel;
        public GameObject SelectorModel => selectorModel;
        public GameObject SelectedModel => selectedModel;
        public GameObject BoardCellModel => boardCellModel;
        public GameObject LoopPrefab => loopPrefab;

        public GameControlItemData GetByType(SelectType selectType)
        {
            foreach (var data in gameControlItemData)
            {
                if (data.ItemType == selectType)
                {
                    return data;
                }
            }

            return null;
        }
    }
}
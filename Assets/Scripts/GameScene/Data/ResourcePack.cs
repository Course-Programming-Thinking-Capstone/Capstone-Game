using System.Collections.Generic;
using UnityEngine;

namespace GameScene.Data
{
    [CreateAssetMenu(fileName = "ControlItem", menuName = "GamePlay/GameControlItemGroupData", order = -999)]
    public class ResourcePack : ScriptableObject
    {
        [SerializeField] private List<GameControlItemData> gameControlItemData;
        [SerializeField] private GameObject targetModel;
        [SerializeField] private GameObject selectorModel;
        [SerializeField] private GameObject selectedModel;
        [SerializeField] private GameObject boardCellModel;
        [SerializeField] private GameObject loopPrefab;
        
        public List<GameControlItemData> GameControlItemData => gameControlItemData;
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
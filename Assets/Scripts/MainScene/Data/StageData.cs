using System;
using System.Collections.Generic;
using UnityEngine;

namespace MainScene.Data
{
    [CreateAssetMenu(fileName = "Data", menuName = "GameModel/StageData", order = 1)]
    public class StageData : ScriptableObject
    {
        [SerializeField] private GameObject modelPrefab;
        [SerializeField] private List<StageItemData> stageItemData;

        public GameObject ModelPrefab
        {
            get => modelPrefab;
            set => modelPrefab = value;
        }
        public List<StageItemData> StageItemData
        {
            get => stageItemData;
            set => stageItemData = value;
        }
    }

    [Serializable]
    public class StageItemData
    {
        [SerializeField] private Sprite render;
        [SerializeField] private string detail;
        [SerializeField] private LevelData dataLevel;

        public Sprite Render
        {
            get => render;
            set => render = value;
        }
        public string Detail
        {
            get => detail;
            set => detail = value;
        }

        public LevelData DataLevel
        {
            get => dataLevel;
            set => dataLevel = value;
        }
    }
}
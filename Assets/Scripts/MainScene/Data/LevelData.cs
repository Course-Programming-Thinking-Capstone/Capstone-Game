using System;
using System.Collections.Generic;
using UnityEngine;

namespace MainScene.Data
{
    [CreateAssetMenu(fileName = "Data", menuName = "GameModel/LevelData", order = 1)]
    public class LevelData : ScriptableObject
    {
        [SerializeField] private GameObject modelPrefab;
        [SerializeField] private List<LevelItemData> levelItemData;

        public GameObject ModelPrefab
        {
            get => modelPrefab;
            set => modelPrefab = value;
        }

        public List<LevelItemData> LevelItemData => levelItemData;
    }

    [Serializable]
    public class LevelItemData
    {
        // setting level
        [SerializeField] private int goldBonus;
        [SerializeField] private int gemBonus;
        [SerializeField] private List<LevelReward> levelReward;
        public List<LevelReward> LevelReward => levelReward;
        public int GoldBonus => goldBonus;
        public int GemBonus => gemBonus;
    }

    [Serializable]
    public class LevelReward
    {
        [SerializeField] private Enums.RewardType rewardType;
        [SerializeField] private int value;
        [SerializeField] private Vector3 boardSize;
        [SerializeField] private Vector3 playerPosition;
        [SerializeField] private Vector3 targetPosition;

        public Enums.RewardType RewardType => rewardType;
        public int Value => value;
    }
}
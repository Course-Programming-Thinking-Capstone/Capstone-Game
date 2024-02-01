using System.Collections.Generic;
using UnityEngine;

namespace GameScene.GameCondition
{
    public class ConditionModel : GameModel
    {
        [SerializeField] private List<Sprite> candySprites;
        [SerializeField] private GameObject conditionPrefab;
        public List<Sprite> CandySprites => candySprites;
        public GameObject ConditionPrefab => conditionPrefab;
    }
}
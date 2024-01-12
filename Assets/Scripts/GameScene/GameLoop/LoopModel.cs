using System.Collections.Generic;
using UnityEngine;

namespace GameScene.GameLoop
{
    public class LoopModel : GameModel
    {
        [SerializeField] private List<Sprite> candySprites;
        [SerializeField] private GameObject loopPrefab;
        public List<Sprite> CandySprites => candySprites;
        public GameObject LoopPrefab => loopPrefab;
    }
}
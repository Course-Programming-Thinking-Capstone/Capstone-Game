using System.Collections.Generic;
using UnityEngine;

namespace GameScene.GameLoop
{
    public class LoopModel : GameModel
    {
        [SerializeField] private List<Sprite> candySprites;
        public List<Sprite> CandySprites => candySprites;
    }
}
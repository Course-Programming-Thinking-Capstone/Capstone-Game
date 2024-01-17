using System.Collections.Generic;
using UnityEngine;

namespace GameScene.GameFunction
{
    public class FuncModel : GameModel
    {
        [SerializeField] private List<Sprite> candySprites;
        public List<Sprite> CandySprites => candySprites;
    }
}
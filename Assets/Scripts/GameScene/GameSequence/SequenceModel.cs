using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameScene.GameSequence
{
    public class SequenceModel : GameModel
    {

        [SerializeField] private List<Sprite> candySprites;
        public List<Sprite> CandySprites => candySprites;
       

       
    }
}
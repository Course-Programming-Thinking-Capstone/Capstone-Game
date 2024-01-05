using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameScene.GameSequence
{
    public class SequenceModel : GameModel
    {
        [SerializeField] private List<ModelSelector> modelSelector;
        [SerializeField] private List<Sprite> candySprites;
  
        [SerializeField] private GameObject candyModel;
        [SerializeField] private GameObject cellModel;
       
        public List<Sprite> CandySprites => candySprites;
        public GameObject CandyModel => candyModel;
        public GameObject CellModel => cellModel;

        public GameObject GetSelector(SelectType selectType)
        {
            var result = modelSelector.FirstOrDefault(o => o.SelectType == selectType);
            return result?.Prefabs;
        }
      
    }

   
}
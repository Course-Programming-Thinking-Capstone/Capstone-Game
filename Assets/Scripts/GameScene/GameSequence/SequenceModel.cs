using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameScene.GameSequence
{
    public class SequenceModel : GameModel
    {
        [SerializeField] private List<ModelSelector> modelSelector;
        [SerializeField] private List<ModelSelected> modelSelected;
        [SerializeField] private List<Sprite> candySprites;
        [SerializeField] private GameObject playerModel;
        [SerializeField] private GameObject candyModel;
        [SerializeField] private GameObject cellModel;
        [SerializeField] private float playerMoveTime;
        [SerializeField] private float blockOffset;

        public GameObject PlayerModel => playerModel;
        public float PlayerMoveTime => playerMoveTime;
        public float BlockOffset => blockOffset;
        public List<Sprite> CandySprites => candySprites;
        public GameObject CandyModel => candyModel;
        public GameObject CellModel => cellModel;

        public GameObject GetSelector(SelectType selectType)
        {
            var result = modelSelector.FirstOrDefault(o => o.SelectType == selectType);
            return result?.Prefabs;
        }

        public GameObject GetSelected(SelectType selectType)
        {
            var result = modelSelected.FirstOrDefault(o => o.SelectType == selectType);
            return result?.Prefabs;
        }
    }

   
}
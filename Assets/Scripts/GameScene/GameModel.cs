using System;
using System.Collections.Generic;
using Data;
using UnityEngine;

namespace GameScene
{
    public class GameModel : MonoBehaviour
    {
        [Header("Overall setting")]
        [SerializeField] private float blockOffset = 1.3f;
        [SerializeField] private float playerMoveTime;
        [SerializeField] private GameObject playerModel;

        [Header("Model Pack")]
        [SerializeField] private ResourcePack resource;
        [SerializeField] private List<Sprite> candySprites;
        public List<Sprite> CandySprites => candySprites;

        #region Getter / Setter

        public ResourcePack Resource => resource;

        public float PlayerMoveTime => playerMoveTime;
        public GameObject PlayerModel => playerModel;

        public virtual float GetBlockOffset()
        {
            return blockOffset;
        }

        #endregion
    }

    [Serializable]
    public class ModelSelect
    {
        [SerializeField] private SelectType selectType;
        [SerializeField] private Sprite renderSpriteSelector;
        [SerializeField] private Sprite renderSpriteSelected;
        public SelectType SelectType => selectType;
        public Sprite RenderSpriteSelector => renderSpriteSelector;
        public Sprite RenderSpriteSelected => renderSpriteSelected;
    }
}
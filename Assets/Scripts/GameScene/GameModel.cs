using System;
using System.Collections.Generic;
using System.Linq;
using GameScene.Data;
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
        // Comment All from here
        [Header("remove soon")]
        [SerializeField] private List<ModelSelect> modelSelect;
        [SerializeField] private GameObject selectorPrefab;
        [SerializeField] private GameObject selectedPrefab;
        [SerializeField] private GameObject targetPrefab;
        [SerializeField] private GameObject cellBoardPrefab;

        #region Getter / Setter

        public ResourcePack Resource => resource;
        public GameObject SelectorPrefab => selectorPrefab;
        public GameObject SelectedPrefab => selectedPrefab;
        public GameObject TargetPrefab => targetPrefab;
        public GameObject CellBoardPrefab => cellBoardPrefab;
        public float PlayerMoveTime => playerMoveTime;
        public GameObject PlayerModel => playerModel;

        public virtual float GetBlockOffset()
        {
            return blockOffset;
        }

        public Sprite GetSelected(SelectType selectType)
        {
            var result = modelSelect.FirstOrDefault(o => o.SelectType == selectType);
            return result?.RenderSpriteSelected;
        }

        public Sprite GetSelector(SelectType selectType)
        {
            var result = modelSelect.FirstOrDefault(o => o.SelectType == selectType);
            return result?.RenderSpriteSelector;
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
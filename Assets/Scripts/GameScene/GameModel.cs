using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameScene
{
    public class GameModel : MonoBehaviour
    {
        [Header("Overall setting")]
        [SerializeField] private float blockOffset = 1.3f;
        [SerializeField] private float playerMoveTime;
        [SerializeField] private GameObject playerModel;

        [Header("Not basic mode")]
        [SerializeField] private List<ModelSelected> modelSelected;
        [SerializeField] private List<ModelSelector> modelSelector;
        [SerializeField] private GameObject selectorPrefab;
        [SerializeField] private GameObject selectedPrefab;
        [SerializeField] private GameObject targetPrefab;
        [SerializeField] private GameObject cellBoardPrefab;

        #region Getter / Setter

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
            var result = modelSelected.FirstOrDefault(o => o.SelectType == selectType);
            return result?.RenderSprite;
        }

        public Sprite GetSelector(SelectType selectType)
        {
            var result = modelSelector.FirstOrDefault(o => o.SelectType == selectType);
            return result?.RenderSprite;
        }

        #endregion
    }

    [Serializable]
    public class ModelSelector
    {
        [SerializeField] private SelectType selectType;
        [SerializeField] private Sprite renderSprite;
        public SelectType SelectType => selectType;
        public Sprite RenderSprite => renderSprite;
    }

    [Serializable]
    public class ModelSelected
    {
        [SerializeField] private SelectType selectType;
        [SerializeField] private Sprite renderSprite;
        public SelectType SelectType => selectType;
        public Sprite RenderSprite => renderSprite;
    }
}
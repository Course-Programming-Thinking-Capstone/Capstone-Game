using System.Collections.Generic;
using DG.Tweening;
using GameScene.Component;
using GameScene.Component.SelectControl;
using GameScene.Data;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace GameScene
{
    public class PadSelectController : MonoBehaviour
    {
        [Header("For animation")]
        [SerializeField] private Button controlButton;
        [SerializeField] private Transform arrowDir;
        [SerializeField] private Transform selectorPad;
        [SerializeField] private Transform selectedPad;
        [SerializeField] private Transform baseXPositionA;
        [SerializeField] private Transform baseXPositionB;
        private bool isClose;

        [Header("For Controller click and drag")]
        [SerializeField] private Transform unSelectContainer;
        [SerializeField] private Transform selectedContainer;
        [SerializeField] private Transform movingContainer;
        [SerializeField] private float partOffset;
        private List<InteractionItem> storeSelected;
        private List<float> tempPosition;
        private bool isDelete;
        private ResourcePack data;
        [CanBeNull] private InteractionItem selectedObject;

        private void Awake()
        {
            storeSelected = new List<InteractionItem>();
            tempPosition = new List<float>();
            controlButton.onClick.AddListener(OnClickOpenClose);
        }

        public void HandleMouseUp()
        {
            if (!selectedObject)
            {
                return;
            }

            isDelete = storeSelected.Count == 15 || IsInDeleteZone();

            if (isDelete) // in delete zone
            {
                SimplePool.Despawn(selectedObject!.gameObject);
                selectedObject = null;
                isDelete = false;
            }
            else // Valid pos
            {
                if (!storeSelected.Contains(selectedObject))
                {
                    storeSelected.Insert(CalculatedNewItemCurrentIndexByPosition(), selectedObject);
                }

                selectedObject!.transform.SetParent(selectedContainer);
                MakeItemSelectedInRightPlace();
                selectedObject = null;
            }
        }

        public void HandleMouseMoveSelected()
        {
            if (!selectedObject)
            {
                return;
            }

            Vector3 mousePos = Input.mousePosition;
            selectedObject!.RectTransform.position = mousePos;

            // isDelete = IsPointInRT(mousePos, deleteZone);
            if (storeSelected.Count == 15)
            {
                return;
            }

            var extensional = CheckInsideLoop();
            if (extensional)
            {
                extensional.MakeItemSelectedInRightPlace();
                extensional.MatchHeightLooper(selectedObject.RectTransform.sizeDelta, true);
            }
            else
            {
                foreach (var selector in storeSelected)
                {
                    if (selector.SelectType == SelectType.Loop)
                    {
                        var extensional1 = (Extensional)selector;
                        extensional1.MakeItemSelectedInRightPlace();
                        extensional1.MatchHeightLooper(Vector2.zero);
                    }
                }
            }

            // check to make space
            MakeEmptySpace();
        }

        /// <summary>
        /// Convert all selected item to basic item 
        /// </summary>
        /// <returns></returns>
        public List<InteractionItem> GetControlPart()
        {
            return storeSelected;
        }

        public void Reset()
        {
            // Clear all things selected
            foreach (var selector in storeSelected)
            {
                SimplePool.Despawn(selector.gameObject);
            }

            storeSelected.Clear();
        }

        public void CreateSelector(List<SelectType> createItems, ResourcePack dataParam)
        {
            data = dataParam;
            // Generate objects selector
            foreach (var o in createItems)
            {
                var obj = Instantiate(data.SelectorModel, unSelectContainer);
                var scriptControl = obj.AddComponent<Basic>();
                scriptControl.Init(OnClickedSelector);
                scriptControl.SelectType = o;
                scriptControl.ChangeRender(data.GetByType(o).UnSelectRender);
            }
        }

        #region Private Control

        private void StoreTempPosition()
        {
            tempPosition.Clear();

            foreach (var item in storeSelected)
            {
                tempPosition.Add(item.RectTransform.position.y);
            }
        }

        private bool IsInDeleteZone()
        {
            return Input.mousePosition.x <= baseXPositionA.position.x;
        }

        /// <summary>
        /// Điểm đầu lấy phân nữa, điểm sau = 1/2 trước đó và 1/2 hiện tại
        /// </summary>
        /// <param name="skipIndex"></param>
        private void MakeItemSelectedInRightPlace(int skipIndex = -1)
        {
            var index = 0;
            var yPos = 0f;

            foreach (var item in storeSelected)
            {
                if (skipIndex == index && selectedObject != null) // place skip
                {
                    yPos -= selectedObject.RectTransform.sizeDelta.y / 2;
                }

                yPos -= index == 0
                    ? item.RectTransform.sizeDelta.y / 2
                    : item.RectTransform.sizeDelta.y / 2 + partOffset;
                item.RectTransform.anchoredPosition = new Vector3(0f, yPos, 0f);
                yPos -= item.RectTransform.sizeDelta.y / 2;
                index++;
            }
        }

        private void MakeEmptySpace()
        {
            if (Input.mousePosition.x > baseXPositionB.position.x || IsInDeleteZone())
            {
                MakeItemSelectedInRightPlace();
                return;
            }

            if (tempPosition.Count > 0)
            {
                var newItemIndex = CalculatedNewItemCurrentIndexByPosition();
                MakeItemSelectedInRightPlace(newItemIndex);
            }
        }

        private int CalculatedNewItemCurrentIndexByPosition()
        {
            var mousePos = Input.mousePosition.y;
            var index = 0;

            foreach (var position in tempPosition)
            {
                if (position > mousePos)
                {
                    index++;
                }
            }

            return index;
        }

        [CanBeNull]
        private Extensional CheckInsideLoop()
        {
            // loop cannot inside loop
            if (selectedObject.SelectType == SelectType.Loop)
            {
                return null;
            }

            var startPosition = (selectedObject.transform.position);
            startPosition.z = -5;
            Ray ray = new Ray(startPosition, Vector3.forward * 100);
            if (Physics.Raycast(ray, out var hit))
            {
                var result = hit.transform.GetComponent<Extensional>();
                return result;
            }

            return null;
        }

        #endregion

        #region CALL BACK

        private void OnClickedSelector(InteractionItem selectedObj)
        {
            // Generate new selected
            if (selectedObj.SelectType == SelectType.Loop)
            {
                var objLoop = SimplePool.Spawn(data.LoopPrefab);
                Extensional looper = objLoop.GetComponent<Extensional>();
                looper.Init(OnClickedSelected);
                looper.SelectType = selectedObj.SelectType;
                // assign to control
                selectedObject = looper;
                selectedObject.transform.SetParent(movingContainer);
            }
            else
            {
                var obj = SimplePool.Spawn(data.SelectedModel);
                // Generate selected Item
                var arrow = obj.GetComponent<Basic>();
                arrow.Init(OnClickedSelected);
                arrow.ChangeRender(data.GetByType(selectedObj.SelectType).SelectedRender);
                arrow.SelectType = selectedObj.SelectType;
                // assign to control
                selectedObject = arrow;
                selectedObject.transform.SetParent(movingContainer);
            }

            StoreTempPosition();
        }

        private void OnClickedSelected(InteractionItem selectedObj)
        {
            // Get object to move
            storeSelected.Remove(selectedObj);
            selectedObject = selectedObj;
            selectedObject!.transform.SetParent(movingContainer);
            StoreTempPosition();
        }

        #endregion

        #region OPEN/CLOSE Control

        private void OnClickOpenClose()
        {
            controlButton.interactable = false;
            if (isClose)
            {
                isClose = false;
                // Open
                OpenAnimation();
            }
            else
            {
                isClose = true;
                // Close
                CloseAnimation();
            }
        }

        private void OpenAnimation()
        {
            arrowDir.DORotate(new Vector3(0, 0, 0), 0.75f).OnComplete(() => { controlButton.interactable = true; });
            selectorPad.DOMoveX(baseXPositionA.position.x, 0.5f);
            selectedPad.DOMoveX(baseXPositionB.position.x, 0.5f);
        }

        private void CloseAnimation()
        {
            arrowDir.DORotate(new Vector3(0, 180, 0), 0.75f).OnComplete(() => { controlButton.interactable = true; });
            selectorPad.DOMoveX(0, 0.5f);
            selectedPad.DOMoveX(0, 0.5f);
        }

        #endregion
    }
}
using System.Collections.Generic;
using System.Linq;
using Data;
using DG.Tweening;
using GameScene.Component;
using GameScene.Component.SelectControl;
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
        [SerializeField] private Transform FuncPad;
        [SerializeField] private Transform baseXPositionA;
        [SerializeField] private Transform baseXPositionB;
        [SerializeField] private Transform baseFuncPosition;
        private bool isClose;

        [Header("For controller click and drag")]
        [SerializeField] private Transform unSelectContainer;
        [SerializeField] private Transform selectedContainer;
        [SerializeField] private Transform movingContainer;
        [SerializeField] private float partOffset;
        [SerializeField] [Range(1, 15)] private int maxBaseControl = 8;
        private List<InteractionItem> storeSelected;
        private List<float> tempPosition;
        private bool isDelete;
        private ResourcePack data;
        [CanBeNull] private InteractionItem selectedObject;

        [Header("Func control")]
        [SerializeField] private Transform funcContainer;
        [SerializeField] [Range(1, 5)]
        private int maxFuncControl = 5;
        private List<InteractionItem> storeFuncSelected = new();
        private List<float> tempFuncPosition = new();

        private void OpenFunc(bool isOpen = true)
        {
            if (isOpen)
            {
                FuncPad.DOMoveY(baseFuncPosition.position.y, 0.5f);
            }
            else
            {
                FuncPad.DOMoveY(0, 0.5f);
            }
        }

        private void Awake()
        {
            storeSelected = new List<InteractionItem>();
            tempPosition = new List<float>();
            controlButton.onClick.AddListener(OnClickOpenClose);
            OpenFunc(false);
        }

        public void SetDisplayPart(InteractionItem effectedItem, bool isOn)
        {
            effectedItem.ActiveEffect(isOn);

            if (storeFuncSelected.Contains(effectedItem))
            {
                effectedItem.transform.SetParent(isOn ? movingContainer : funcContainer);
            }
            else
            {
                effectedItem.transform.SetParent(isOn ? movingContainer : selectedContainer);
            }
        }

        public void HandleMouseUp()
        {
            if (!selectedObject)
            {
                return;
            }

            var checker = CheckInsideOther();
            isDelete = IsInDeleteZone();
            if (isDelete) // in delete zone
            {
                SimplePool.Despawn(selectedObject!.gameObject);
                selectedObject = null;
                isDelete = false;
            }
            else if (checker == null) // normal add
            {
                if (storeSelected.Count == maxBaseControl) //max
                {
                    SimplePool.Despawn(selectedObject!.gameObject);
                    selectedObject = null;
                    isDelete = false;
                }
                else
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
            else if (checker!.CompareTag(Constants.LoopTag))
            {
                var extensional = checker.GetComponent<Extensional>();
                extensional.AddItem(selectedObject);
                MakeItemSelectedInRightPlace();
                selectedObject = null;
            }
            else if (checker.CompareTag(Constants.FuncTag))
            {
                if (storeFuncSelected.Count == maxFuncControl) //max
                {
                    SimplePool.Despawn(selectedObject!.gameObject);
                    selectedObject = null;
                    isDelete = false;
                }
                else
                {
                    storeFuncSelected.Insert(CalculatedNewItemCurrentIndexByPosition(true), selectedObject);
                    selectedObject!.transform.SetParent(funcContainer);
                    MakeItemSelectedInRightPlace(isFunc: true);
                    selectedObject = null;
                }
            }

            if (storeSelected.Any(o => o.SelectType == SelectType.Func))
            {
                OpenFunc();
            }
            else
            {
                OpenFunc(false);
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

            if (storeSelected.Count == 15)
            {
                return;
            }

            var checker = CheckInsideOther();
            if (checker == null) // Normal behavior
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

                // check to make space
                MakeEmptySpace(false);
            }
            else if (checker.CompareTag(Constants.LoopTag))
            {
                var extensional = checker.GetComponent<Extensional>();
                extensional.MakeEmptySpace(selectedObject.RectTransform);
                extensional.MatchHeightLooper(selectedObject.RectTransform.sizeDelta / 2, true);
            }
            else if (checker.CompareTag(Constants.FuncTag))
            {
                // check to make space
                foreach (var selector in storeSelected)
                {
                    if (selector.SelectType == SelectType.Loop)
                    {
                        var extensional1 = (Extensional)selector;
                        extensional1.MakeItemSelectedInRightPlace();
                        extensional1.MatchHeightLooper(Vector2.zero);
                    }
                }

                MakeEmptySpace(true);
            }
        }

        /// <summary>
        /// Convert all selected item to basic item 
        /// </summary>
        /// <returns></returns>
        public List<InteractionItem> GetControlPart()
        {
            var result = new List<InteractionItem>();
            foreach (var baseSelectItem in storeSelected)
            {
                switch (baseSelectItem.SelectType)
                {
                    case SelectType.Up:
                    case SelectType.Down:
                    case SelectType.Left:
                    case SelectType.Right:
                    case SelectType.Collect:
                        result.Add(baseSelectItem);
                        break;
                    case SelectType.Loop:
                        result.Add(baseSelectItem);
                        var loop = (Extensional)baseSelectItem;
                        for (int i = 0; i < loop.LoopCount; i++)
                        {
                            result.AddRange(loop.StoreSelected);
                        }

                        break;
                    case SelectType.Func:
                        result.AddRange(storeFuncSelected);

                        break;
                    case SelectType.Condition:
                        break;
                }
            }

            return result;
        }

        public void Reset()
        {
            // Clear all things selected
            foreach (var selector in storeSelected)
            {
                if (selector.SelectType == SelectType.Loop)
                {
                    var loop = (Extensional)selector;
                    foreach (var selectorInLoop in loop.StoreSelected)
                    {
                        SimplePool.Despawn(selectorInLoop.gameObject);
                    }
                }

                SimplePool.Despawn(selector.gameObject);
            }

            foreach (var selector in storeFuncSelected)
            {
                SimplePool.Despawn(selector.gameObject);
            }

            storeSelected.Clear();
            storeFuncSelected.Clear();
            OpenFunc(false);
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

            tempFuncPosition.Clear();

            foreach (var item in storeFuncSelected)
            {
                tempFuncPosition.Add(item.RectTransform.position.y);
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
        private void MakeItemSelectedInRightPlace(int skipIndex = -1, bool isFunc = false)
        {
            var index = 0;
            var yPos = 0f;
            foreach (var item in storeSelected)
            {
                if (skipIndex == index && selectedObject != null && !isFunc) // place skip
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

            index = 0;
            yPos = 0f;
            foreach (var item in storeFuncSelected)
            {
                if (skipIndex == index && selectedObject != null && isFunc) // place skip
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

        private void MakeEmptySpace(bool isFunc = false)
        {
            // check for outside
            if (Input.mousePosition.x > baseXPositionB.position.x || IsInDeleteZone())
            {
                MakeItemSelectedInRightPlace();
                return;
            }

            if (tempPosition.Count > 0 && !isFunc && tempPosition.Count < maxBaseControl)
            {
                var newItemIndex = CalculatedNewItemCurrentIndexByPosition(false);
                MakeItemSelectedInRightPlace(newItemIndex);
            }
            else if (tempFuncPosition.Count > 0 && isFunc && storeFuncSelected.Count < maxFuncControl)
            {
                var newItemIndex = CalculatedNewItemCurrentIndexByPosition(true);
                MakeItemSelectedInRightPlace(newItemIndex, true);
            }
        }

        private int CalculatedNewItemCurrentIndexByPosition(bool isFunc = false)
        {
            var mousePos = Input.mousePosition.y;
            var index = 0;
            if (isFunc)
            {
                foreach (var position in tempFuncPosition)
                {
                    if (position > mousePos)
                    {
                        index++;
                    }
                }
            }
            else
            {
                foreach (var position in tempPosition)
                {
                    if (position > mousePos)
                    {
                        index++;
                    }
                }
            }

            return index;
        }

        [CanBeNull]
        private Transform CheckInsideOther()
        {
            // loop cannot inside loop
            if (selectedObject == null)
            {
                return null;
            }

            if (selectedObject.SelectType == SelectType.Loop
                || selectedObject.SelectType == SelectType.Func
               )
            {
                return null;
            }

            var startPosition = (selectedObject.transform.position);
            startPosition.z = -5;
            Ray ray = new Ray(startPosition, Vector3.forward * 100);
            if (Physics.Raycast(ray, out var hit))
            {
                if (hit.transform.CompareTag(Constants.LoopTag) || hit.transform.CompareTag(Constants.FuncTag))
                {
                    return hit.transform;
                }
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
                objLoop.transform.SetParent(selectedContainer);
                objLoop.transform.localScale = Vector3.one;
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
                obj.transform.SetParent(selectedContainer);
                obj.transform.localScale = Vector3.one;
                // assign to control
                selectedObject = arrow;
                selectedObject.transform.SetParent(movingContainer);
            }

            StoreTempPosition();
        }

        private void OnClickedSelected(InteractionItem selectedObj)
        {
            // Get object to move
            // not have?
            storeSelected.Remove(selectedObj);
            storeFuncSelected.Remove(selectedObj);
            foreach (var selector in storeSelected)
            {
                if (selector.SelectType == SelectType.Loop)
                {
                    var looper = (Extensional)selector;
                    looper.RemoveItem(selectedObj);
                }
            }

            // Get object to move
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
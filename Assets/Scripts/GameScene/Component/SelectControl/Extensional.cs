using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Utilities;

namespace GameScene.Component.SelectControl
{
    public class Extensional : InteractionItem, IPointerDownHandler
    {
        [Header("Extensional behavior")]
        [SerializeField] private RectTransform container;
        [SerializeField] private BoxCollider boxCollider;
        [SerializeField] private float offSetHeight;
        [SerializeField] private float partOffSet = 0.2f;
        public List<InteractionItem> StoreSelected { get; } = new();
        private readonly List<float> tempPosition = new();

        private float baseHeight;

        [Header("Looper behavior")]
        [SerializeField] private int maxLoop = 8;
        [SerializeField] private TextMeshProUGUI loopTxt;
        public int LoopCount { get; private set; } = 2;

        public override void Init(UnityAction<InteractionItem> onClickParam)
        {
            base.Init(onClickParam);
            if (loopTxt != null)
            {
                LoopCount = 2;
                loopTxt.text = LoopCount.ToString();
            }

            if (baseHeight == 0f)
            {
                baseHeight = rectTransform.sizeDelta.y;
            }

            foreach (var item in StoreSelected)
            {
                SimplePool.Despawn(item.gameObject);
            }

            StoreSelected.Clear();
            MakeItemSelectedInRightPlace();
            MatchHeightLooper(Vector2.zero);
            StoreTempPosition();
        }

        // Control

        #region UI Control

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

        private void StoreTempPosition()
        {
            tempPosition.Clear();
            foreach (var item in StoreSelected)
            {
                tempPosition.Add(item.RectTransform.position.y);
            }
        }

        public void MatchHeightLooper(Vector2 itemSize, bool makeSpace = false)
        {
            var temp = baseHeight;
            var tempVec = rectTransform.sizeDelta;
            if (StoreSelected.Count == 0)
            {
                tempVec.y = baseHeight;
                rectTransform.sizeDelta = tempVec;
                boxCollider.size = tempVec;
                return;
            }

            foreach (var item in StoreSelected)
            {
                temp += item.RectTransform.sizeDelta.y + partOffSet;
            }

            if (makeSpace) // add fake size
            {
                temp += itemSize.y;
            }

            temp += offSetHeight;
            tempVec.y = temp;
            rectTransform.sizeDelta = tempVec;
            boxCollider.size = tempVec;
        }

        public void MakeEmptySpace(RectTransform selectedObj)
        {
            if (tempPosition.Count > 0)
            {
                var newItemIndex = CalculatedNewItemCurrentIndexByPosition();
                MakeItemSelectedInRightPlace(selectedObj, newItemIndex);
            }
        }

        /// <summary>
        /// Điểm đầu lấy phân nữa, điểm sau = 1/2 trước đó và 1/2 hiện tại
        /// </summary>
        /// <param name="selectedObj"></param>
        /// <param name="skipIndex"></param>
        public void MakeItemSelectedInRightPlace(RectTransform selectedObj = null, int skipIndex = -1)
        {
            var index = 0;
            var yPos = 0f;

            foreach (var item in StoreSelected)
            {
                if (skipIndex == index && selectedObj != null) // place skip
                {
                    yPos -= selectedObj.sizeDelta.y / 2;
                }

                yPos -= index == 0
                    ? item.RectTransform.sizeDelta.y / 2
                    : item.RectTransform.sizeDelta.y / 2 + partOffSet;
                item.RectTransform.anchoredPosition = new Vector3(0f, yPos, 0f);
                yPos -= item.RectTransform.sizeDelta.y / 2;
                index++;
            }
        }

        #endregion

        #region Controller

        public void AddItem(InteractionItem interactionItemItem)
        {
            if (!StoreSelected.Contains(interactionItemItem))
            {
                interactionItemItem.transform.SetParent(container);
                StoreSelected.Insert(
                    CalculatedNewItemCurrentIndexByPosition()
                    , interactionItemItem);
            }

            MakeItemSelectedInRightPlace();
            MatchHeightLooper(interactionItemItem.RectTransform.sizeDelta);
            StoreTempPosition();
        }

        public void RemoveItem(InteractionItem interactionItemItem)
        {
            StoreSelected.Remove(interactionItemItem);
            MakeItemSelectedInRightPlace();
            MatchHeightLooper(interactionItemItem.RectTransform.sizeDelta);
            StoreTempPosition();
        }

        public override void ChangeRender(Sprite newRender)
        {
            base.ChangeRender(newRender);
            rendererImg.SetNativeSize();
        }

        #endregion

        public void OnPointerDown(PointerEventData eventData)
        {
            OnClickButton();
        }

        /// <summary>
        /// Clicked loop number
        /// </summary>
        public void OnClickLoop()
        {
            if (LoopCount == maxLoop)
            {
                LoopCount = 2;
            }
            else
            {
                LoopCount++;
            }

            loopTxt.text = LoopCount.ToString();
        }
    }
}
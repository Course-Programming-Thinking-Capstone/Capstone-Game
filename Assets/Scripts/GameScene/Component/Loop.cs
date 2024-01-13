using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace GameScene.Component
{
    public class Loop : Selector, IPointerDownHandler
    {
        [SerializeField] private int maxLoop = 8;
        [SerializeField] private TextMeshProUGUI loopTxt;
        public int LoopCount { get; set; } = 2;
        [Header("Loop behavior")]
        [SerializeField] private RectTransform container;
        [SerializeField] private BoxCollider boxCollider;
        [SerializeField] private float offSetHeight;
        private readonly List<Selector> storeSelected = new();
        private readonly List<RectTransform> storeRect = new();
        private readonly List<Vector2> storedPosition = new();
        private readonly float offSet = 0.2f;
        private float baseHeight;

        public override void Init(UnityAction<Selector> onClickParam)
        {
            base.Init(onClickParam);
            LoopCount = 2;
            loopTxt.text = LoopCount.ToString();
            if (baseHeight == 0f)
            {
                baseHeight = rectTransform.sizeDelta.y;
            }

            foreach (var item in storeSelected)
            {
                SimplePool.Despawn(item.gameObject);
            }

            storeSelected.Clear();
            storeRect.Clear();
            ReSortItemsSelected(storeRect);
            FixHeightLooper(Vector2.zero);
            FixBoxCollider();
            StoreTempPosition();
        }

        // Control

        #region UI Control

        private int CalculatedCurrentPosition(Vector2 mousePos)
        {
            for (int i = 0; i < storedPosition.Count; i++)
            {
                if (i == 0 && storedPosition[i].y - offSet < mousePos.y) // first item
                {
                    return 0;
                }

                if (i == storedPosition.Count - 1) // last item
                {
                    return storedPosition.Count;
                }

                if (storedPosition[i].y + offSet > mousePos.y
                    && storedPosition[i + 1].y - offSet < mousePos.y)
                {
                    return i + 1;
                }
            }

            return storedPosition.Count;
        }

        private void SetPositionSelected(RectTransform item, float yPosition)
        {
            item.anchoredPosition = new Vector3(container.anchoredPosition.x, yPosition, 0f);
        }

        private void StoreTempPosition()
        {
            storedPosition.Clear();
            foreach (var item in storeSelected)
            {
                storedPosition.Add(item.RectTransform.position);
            }
        }

        private void FixBoxCollider()
        {
            boxCollider.size = rectTransform.sizeDelta;
        }

        public void FixHeightLooper(Vector2 itemSize, bool makeSpace = false)
        {
            var temp = rectTransform.sizeDelta;
            temp.y = baseHeight;
            for (int i = 0; i < storeSelected.Count; i++)
            {
                if (i == 0)
                {
                    temp.y = baseHeight + (storeSelected[i].RectTransform.sizeDelta.y - offSetHeight);
                }
                else
                {
                    temp.y += storeSelected[i].RectTransform.sizeDelta.y;
                }
            }

            if (makeSpace) // add fake size
            {
                if (storeSelected.Count == 0)
                {
                    temp.y = baseHeight + (itemSize.y - offSetHeight);
                }
                else
                {
                    temp.y += (itemSize.y);
                }
            }

            rectTransform.sizeDelta = temp;
        }

        private void ReSortItemsSelected(IReadOnlyList<RectTransform> items)
        {
            var yPosition = 0f;
            foreach (var rectItem in items)
            {
                yPosition += -rectItem.sizeDelta.y / 2;
                SetPositionSelected(rectItem, yPosition);
                yPosition += -rectItem.sizeDelta.y / 2;
            }
        }

        public void MakeEmptySpace(RectTransform itemSpace)
        {
            var yPosition = 0f;
            int index = CalculatedCurrentPosition(itemSpace.position);

            for (int i = 0; i < storeRect.Count; i++)
            {
                yPosition += -storeRect[i].sizeDelta.y / 2;
                if (index == i)
                {
                    yPosition += -itemSpace.sizeDelta.y;
                }

                SetPositionSelected(storeRect[i], yPosition);
                yPosition += -storeRect[i].sizeDelta.y / 2;
            }
        }

        public void ClearEmptySpace()
        {
            var yPosition = 0f;
            foreach (var rectItem in storeRect)
            {
                yPosition += -rectItem.sizeDelta.y / 2;
                SetPositionSelected(rectItem, yPosition);
                yPosition += -rectItem.sizeDelta.y / 2;
            }

            StoreTempPosition();
        }

        #endregion

        #region Controller

        public void AddItem(Selector selectorItem)
        {
            if (!storeSelected.Contains(selectorItem))
            {
                selectorItem.transform.SetParent(container);
                var position = selectorItem.RectTransform.position;
                storeRect.Insert(
                    CalculatedCurrentPosition(position)
                    , selectorItem.RectTransform);
                storeSelected.Insert(
                    CalculatedCurrentPosition(position)
                    , selectorItem);
            }

            ReSortItemsSelected(storeRect);
            FixHeightLooper(selectorItem.RectTransform.sizeDelta);
            FixBoxCollider();
            StoreTempPosition();
        }

        public void RemoveItem(Selector selectorItem)
        {
            storeSelected.Remove(selectorItem);
            storeRect.Remove(selectorItem.RectTransform);
            ReSortItemsSelected(storeRect);
            FixHeightLooper(selectorItem.RectTransform.sizeDelta);
            FixBoxCollider();
            StoreTempPosition();
        }

        public override void ChangeRender(Sprite newRender)
        {
            base.ChangeRender(newRender);
            renderer.SetNativeSize();
        }

        #endregion

        public void OnPointerDown(PointerEventData eventData)
        {
            OnClickButton();
        }

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
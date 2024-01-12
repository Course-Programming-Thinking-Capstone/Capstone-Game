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
        private readonly List<Selector> storeSelected = new();
        private readonly List<RectTransform> storeRect = new();

        // Control

        #region Control inside

        public void AddItem(Selector selectorItem)
        {
            selectorItem.transform.SetParent(container);
            storeSelected.Add(selectorItem);
            storeRect.Add(selectorItem.RectTransform);
            ReSortItemsSelected(storeRect);
            StoreTempPosition();
        }

        public void ReSortItemsSelected(List<RectTransform> items)
        {
            var yPosition = 0f;
            for (int i = 0; i < items.Count; i++)
            {
                yPosition += -items[i].sizeDelta.y / 2;
                SetPositionSelected(items[i], yPosition);
                yPosition += -items[i].sizeDelta.y / 2;
            }
        }

        private readonly List<Vector2> storedPosition = new();

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

        private readonly float offSet = 0.2f;

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
            item.anchoredPosition = new Vector3(0f, yPosition, 0f);
        }

        private void StoreTempPosition()
        {
            storedPosition.Clear();
            foreach (var item in storeSelected)
            {
                storedPosition.Add(item.RectTransform.position);
            }
        }

        #endregion

        public override void Init(UnityAction<Selector> onClickParam)
        {
            base.Init(onClickParam);
            LoopCount = 2;
            loopTxt.text = LoopCount.ToString();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            OnClickButton();
        }

        public override void ChangeRender(Sprite newRender)
        {
            base.ChangeRender(newRender);
            renderer.SetNativeSize();
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
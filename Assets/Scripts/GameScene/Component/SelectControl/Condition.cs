using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Utilities;

namespace GameScene.Component.SelectControl
{
    public class Condition : InteractionItem, IPointerDownHandler
    {
        [Header("Condition behavior")]
        [SerializeField] private RectTransform container;
        [SerializeField] private BoxCollider boxCollider;
        [SerializeField] private float offSetHeight;
        public List<InteractionItem> StoreSelected { get; } = new();
        private readonly List<RectTransform> storeRect = new();
        private readonly List<Vector2> storedPosition = new();
        private readonly float offSet = 0.2f;
        private float baseHeight;
        
        public override void Init(UnityAction<InteractionItem> onClickParam)
        {
            base.Init(onClickParam);
            if (baseHeight == 0f)
            {
                baseHeight = rectTransform.sizeDelta.y;
            }

            foreach (var item in StoreSelected)
            {
                SimplePool.Despawn(item.gameObject);
            }

            StoreSelected.Clear();
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
            foreach (var item in StoreSelected)
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
            for (int i = 0; i < StoreSelected.Count; i++)
            {
                if (i == 0)
                {
                    temp.y = baseHeight + (StoreSelected[i].RectTransform.sizeDelta.y - offSetHeight);
                }
                else
                {
                    temp.y += StoreSelected[i].RectTransform.sizeDelta.y;
                }
            }

            if (makeSpace) // add fake size
            {
                if (StoreSelected.Count == 0)
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

        public void AddItem(InteractionItem interactionItemItem)
        {
            if (!StoreSelected.Contains(interactionItemItem))
            {
                interactionItemItem.transform.SetParent(container);
                var position = interactionItemItem.RectTransform.position;
                storeRect.Insert(
                    CalculatedCurrentPosition(position)
                    , interactionItemItem.RectTransform);
                StoreSelected.Insert(
                    CalculatedCurrentPosition(position)
                    , interactionItemItem);
            }

            ReSortItemsSelected(storeRect);
            FixHeightLooper(interactionItemItem.RectTransform.sizeDelta);
            FixBoxCollider();
            StoreTempPosition();
        }

        public void RemoveItem(InteractionItem interactionItemItem)
        {
            StoreSelected.Remove(interactionItemItem);
            storeRect.Remove(interactionItemItem.RectTransform);
            ReSortItemsSelected(storeRect);
            FixHeightLooper(interactionItemItem.RectTransform.sizeDelta);
            FixBoxCollider();
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

  
    }
}
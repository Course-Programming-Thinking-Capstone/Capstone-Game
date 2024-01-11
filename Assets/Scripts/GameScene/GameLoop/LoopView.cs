using System.Collections.Generic;
using UnityEngine;

namespace GameScene.GameLoop
{
    public class LoopView : GameView
    {
        [Header("Canvas references")]
        [SerializeField] private Transform selectorContainer;
        [SerializeField] private Transform selectedContainer;
        [SerializeField] private Transform movingContainer;

        #region Canvas

        public void SetParentSelector(Transform child)
        {
            child.SetParent(selectorContainer);
            child.localScale = Vector3.one;
        }

        public void SetParentSelected(Transform child)
        {
            child.SetParent(selectedContainer);
            child.localScale = Vector3.one;
        }

        public void SetParentSelectedToMove(Transform child)
        {
            child.SetParent(movingContainer);
            child.localScale = Vector3.one;
        }

        /// <summary>
        /// Add and object to selected list with index
        /// </summary>
        private void SetPositionSelected(RectTransform item, int index)
        {
            var yPosition = -item.sizeDelta.y * (index - 0.5f);
            item.anchoredPosition = new Vector3(0f, yPosition, 0f);
        }

        /// <summary>
        /// Sorted all selected object to its position
        /// </summary>
        /// <param name="items"></param>
        public void ReSortItemsSelected(List<RectTransform> items)
        {
            for (int i = 0; i < items.Count; i++)
            {
                SetPositionSelected(items[i], i + 1);
            }
        }

        public void MakeEmptySpace(List<RectTransform> items, int indexToMakeSpace)
        {
            var itemIndex = 1;
            for (int i = 0; i < items.Count; i++)
            {
                if (indexToMakeSpace == i)
                {
                    itemIndex++;
                }

                SetPositionSelected(items[i], itemIndex);
                itemIndex++;
            }
        }

        #endregion
    }
}
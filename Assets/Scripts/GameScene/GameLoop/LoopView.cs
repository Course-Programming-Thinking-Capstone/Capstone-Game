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
        private void SetPositionSelected(RectTransform item, float yPosition)
        {
            item.anchoredPosition = new Vector3(0f, yPosition, 0f);
        }

        /// <summary>
        /// Sorted all selected object to its position
        /// </summary>
        /// <param name="items"></param>
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

        public void MakeEmptySpace(List<RectTransform> items, int indexToMakeSpace, float sizeSpace)
        {
            var yPosition = 0f;
            for (int i = 0; i < items.Count; i++)
            {
                yPosition += -items[i].sizeDelta.y / 2;
                if (indexToMakeSpace == i)
                {
                    yPosition += -sizeSpace;
                }

                SetPositionSelected(items[i], yPosition);
                yPosition += -items[i].sizeDelta.y / 2;
            }
        }
        public void InitGroundBoardFakePosition(Vector2 board, float offSet)
        {
            boardSize = board;
            var sizeY = (int)board.y;
            var sizeX = (int)board.x;
            for (int i = 0; i < sizeY; i++) // vertical
            {
                for (int j = 0; j < sizeX; j++) // horizontal
                {
                    var positionNew = startGroundPosition.position;
                    positionNew.x += offSet * j;
                    positionNew.y += offSet * i;
                    positions.Add(positionNew);
                }
            }
        }
        #endregion
    }
}
using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace GameScene
{
    public class GameView : MonoBehaviour
    {
        [SerializeField] private Transform selectorContainer;
        [SerializeField] private Transform selectedContainer;
        [SerializeField] private RectTransform playZone;
        private RectTransform playerRectTransform;
        private float cellXSize;
        private float cellYSize;

        public void MovePlayer(List<SelectType> selectTypes, float moveTime)
        {
            Vector2 currentPosition = playerRectTransform.anchoredPosition;
            int currentStep = 0;
            MoveToNextStep(selectTypes, currentPosition, currentStep, moveTime);
        }

        private void MoveToNextStep(List<SelectType> selectTypes, Vector2 currentPosition, int currentStep, float moveTime)
        {
            if (currentStep < selectTypes.Count)
            {
                Vector2 moveDirection = Vector2.zero;
                switch (selectTypes[currentStep])
                {
                    case SelectType.None:
                        break;
                    case SelectType.Up:
                        moveDirection = Vector2.up * cellYSize;
                        break;
                    case SelectType.Down:
                        moveDirection = Vector2.down * cellYSize;
                        break;
                    case SelectType.Left:
                        moveDirection = Vector2.left * cellXSize;
                        break;
                    case SelectType.Right:
                        moveDirection = Vector2.right * cellXSize;
                        break;
                }

                // Tính vị trí mới
                Vector2 targetPosition = currentPosition + moveDirection;
                
                playerRectTransform.DOAnchorPos(targetPosition, moveTime).SetEase(Ease.Linear).OnComplete(() =>
                {
                    MoveToNextStep(selectTypes, targetPosition, currentStep + 1, moveTime);
                });
            }
        }

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

        public void SetPositionSelected(RectTransform item, int index)
        {
            var yPosition = -item!.sizeDelta.y * (index - 0.5f);
            item!.anchoredPosition = new Vector3(0f, yPosition, 0f);
        }

        public void ReSortItemsSelected(List<RectTransform> items)
        {
            for (int i = 0; i < items.Count; i++)
            {
                SetPositionSelected(items[i], i + 1);
            }
        }

        public void MakeEmptySpace(List<RectTransform> items, int indexToMakeSpace)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (indexToMakeSpace == i)
                {
                    continue;
                }

                SetPositionSelected(items[i], i + 1);
            }
        }

        public void InitPlayerPosition(RectTransform playerTransform, Vector2 map, Vector2 playerPos)
        {
            playerRectTransform = playerTransform;
            playerTransform.SetParent(playZone);
            // Get the rectangular bounding box of your UI element
            var rect = playZone.rect;
            var anchoredPosition = playZone.anchoredPosition;

            cellXSize = rect.width / map.x;

            cellYSize = rect.height / map.x;

            var lefBottomSize =
                new Vector2(anchoredPosition.x - rect.width / 2f, anchoredPosition.y - rect.height / 2f);

            var playerPosToSet = lefBottomSize;
            playerPosToSet.x += cellXSize * (playerPos.x - 0.5f);
            playerPosToSet.y += cellYSize * (playerPos.y - 0.5f);
            playerTransform.anchoredPosition = playerPosToSet;
        }
    }
}
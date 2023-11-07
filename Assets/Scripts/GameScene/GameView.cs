using System.Collections.Generic;
using DG.Tweening;
using Spine.Unity;
using UnityEngine;

namespace GameScene
{
    public class GameView : MonoBehaviour
    {
        [SerializeField] private Transform selectorContainer;
        [SerializeField] private Transform selectedContainer;
        [SerializeField] private RectTransform playZone;
        [SerializeField] private string idleAnimation;
        [SerializeField] private string moveAnimation;
        [SerializeField] private string collectAnimation;
        private RectTransform playerRectTransform;
        private SkeletonGraphic playerSkeletonGraphic;
        private float cellXSize;
        private float cellYSize;

        public void MovePlayer(List<SelectType> selectTypes, float moveTime)
        {
            Vector2 currentPosition = playerRectTransform.anchoredPosition;
            Quaternion currentRotation = playerRectTransform.rotation; // Lấy góc quay hiện tại
            int currentStep = 0;
            if (!playerSkeletonGraphic)
            {
                playerSkeletonGraphic = playerRectTransform.GetComponent<SkeletonGraphic>();
            }

            MoveToNextStep(selectTypes, currentPosition, currentRotation, currentStep, moveTime);
        }

        private void MoveToNextStep(List<SelectType> selectTypes, Vector2 currentPosition, Quaternion currentRotation,
            int currentStep, float moveTime)
        {
            if (currentStep < selectTypes.Count)
            {
                Vector2 moveDirection = Vector2.zero;
                Quaternion targetRotation = currentRotation;

                switch (selectTypes[currentStep])
                {
                    case SelectType.None:
                        break;
                    case SelectType.Up:
                        playerSkeletonGraphic.AnimationState.SetAnimation(0, moveAnimation, true);
                        moveDirection = Vector2.up * cellYSize;
                        break;
                    case SelectType.Down:
                        playerSkeletonGraphic.AnimationState.SetAnimation(0, moveAnimation, true);
                        moveDirection = Vector2.down * cellYSize;
                        break;
                    case SelectType.Left:
                        playerSkeletonGraphic.AnimationState.SetAnimation(0, moveAnimation, true);
                        moveDirection = Vector2.left * cellXSize;
                        targetRotation = Quaternion.Euler(0, 0, 0);
                        break;
                    case SelectType.Right:
                        playerSkeletonGraphic.AnimationState.SetAnimation(0, moveAnimation, true);
                        moveDirection = Vector2.right * cellXSize;
                        targetRotation = Quaternion.Euler(0, 180, 0);
                        break;
                    case SelectType.Collect:
                        playerSkeletonGraphic.AnimationState.SetAnimation(0, collectAnimation, true);
                        moveDirection = Vector2.zero; // not move
                        break;
                }

                Vector2 targetPosition = currentPosition + moveDirection;
                Sequence sequence = DOTween.Sequence();
                sequence.Append(playerRectTransform.DOAnchorPos(targetPosition, moveTime));
                sequence.Join(playerRectTransform.DORotateQuaternion(targetRotation, moveTime / 2));
                sequence.OnComplete(() =>
                {
                    MoveToNextStep(selectTypes, targetPosition, targetRotation, currentStep + 1, moveTime);
                });
            }
            else
            {
                playerSkeletonGraphic.AnimationState.SetAnimation(0, idleAnimation, true);
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

        public void InitPlayerPosition(RectTransform playerTransform, Vector2 map, Vector2 playerPos)
        {
            playerRectTransform = playerTransform;
            playerTransform.SetParent(playZone);
            // Get the rectangular bounding box of your UI element
            var rect = playZone.rect;
            var anchoredPosition = playZone.anchoredPosition;

            cellXSize = rect.width / map.x;
            cellYSize = rect.height / map.y;

            var lefBottomSize =
                new Vector2(anchoredPosition.x - rect.width / 2f, anchoredPosition.y - rect.height / 2f);

            var playerPosToSet = lefBottomSize;
            playerPosToSet.x += cellXSize * (playerPos.x - 0.5f);
            playerPosToSet.y += cellYSize * (playerPos.y - 0.7f);
            playerTransform.anchoredPosition = playerPosToSet;
        }

        public void InitCandyPosition(RectTransform candy, Vector2 playerPos)
        {
            candy.SetParent(playZone);
            // Get the rectangular bounding box of your UI element
            var rect = playZone.rect;
            var anchoredPosition = playZone.anchoredPosition;
            var lefBottomSize =
                new Vector2(anchoredPosition.x - rect.width / 2f, anchoredPosition.y - rect.height / 2f);
            var playerPosToSet = lefBottomSize;
            playerPosToSet.x += cellXSize * (playerPos.x - 0.4f);
            playerPosToSet.y += cellYSize * (playerPos.y - 0.5f);
            candy.anchoredPosition = playerPosToSet;
        }
    }
}
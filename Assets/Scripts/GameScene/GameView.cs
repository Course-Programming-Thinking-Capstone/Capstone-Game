using System.Collections.Generic;
using DG.Tweening;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Events;

namespace GameScene
{
    public class GameView : MonoBehaviour
    {
        [Header("2D references")]
        [SerializeField] private Transform container;
        [SerializeField] private Transform startGroundPosition;
        [SerializeField] private Transform blockContainer;

        [Header("Canvas references")]
        [SerializeField] private Transform selectorContainer;
        [SerializeField] private Transform selectedContainer;

        [SerializeField] private string idleAnimation;
        [SerializeField] private string moveAnimation;
        [SerializeField] private string collectAnimation;

        [Header("Cache")]
        private List<Transform> groundPosition;
        private Transform playerRectTransform;
        private SkeletonAnimation playerSkeleton;
        // private float cellXSize;
        // private float cellYSize;
        private Vector2 boardSize;
        private Vector2 playerBasePosition;

        #region Initialize

        public void InitPlayerPosition(Transform playerTransform, Vector2 playerPos)
        {
            playerRectTransform = playerTransform;
            playerBasePosition = playerPos;
            playerTransform.SetParent(container);
            playerTransform.rotation = Quaternion.Euler(0, 180, 0);
            playerTransform.position = GetPositionFromBoard(playerPos);
        }

        public void InitTargetPosition(Transform target, Vector2 objPosition)
        {
            target.SetParent(container);
            target.position = GetPositionFromBoard(objPosition);
        }

        public void InitGroundBoard(List<Transform> groundItems, Vector2 board, float offSet)
        {
            groundPosition = groundItems;
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
                    groundItems[i * sizeX + j].position = positionNew;
                    groundItems[i * sizeX + j].SetParent(blockContainer);
                }
            }
        }

        #endregion

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

        #endregion

        #region 2D

        public void MovePlayer(List<SelectType> selectTypes, float moveTime, UnityAction onFail)
        {
            var currentStep = 0;
            if (!playerSkeleton)
            {
                playerSkeleton = playerRectTransform.GetComponent<SkeletonAnimation>();
            }

            MoveToNextStep(selectTypes, playerBasePosition, playerRectTransform.rotation, currentStep, moveTime,
                onFail);
        }

        private void MoveToNextStep(List<SelectType> selectTypes, Vector2 currentPosition, Quaternion currentRotation,
            int currentStep, float moveTime, UnityAction onFail)
        {
            if (currentStep < selectTypes.Count)
            {
                var moveDirection = Vector2.zero;
                var targetRotation = currentRotation;
                switch (selectTypes[currentStep])
                {
                    case SelectType.None:
                        break;
                    case SelectType.Up:
                        playerSkeleton.AnimationState.SetAnimation(0, moveAnimation, true);
                        moveDirection = Vector2.up;
                        break;
                    case SelectType.Down:
                        playerSkeleton.AnimationState.SetAnimation(0, moveAnimation, true);
                        moveDirection = Vector2.down;
                        break;
                    case SelectType.Left:
                        playerSkeleton.AnimationState.SetAnimation(0, moveAnimation, true);
                        moveDirection = Vector2.left;
                        targetRotation = Quaternion.Euler(0, 0, 0);
                        break;
                    case SelectType.Right:
                        playerSkeleton.AnimationState.SetAnimation(0, moveAnimation, true);
                        moveDirection = Vector2.right;
                        targetRotation = Quaternion.Euler(0, 180, 0);
                        break;
                    case SelectType.Collect:
                        playerSkeleton.AnimationState.SetAnimation(0, collectAnimation, true);
                        moveDirection = Vector2.zero; // not move
                        break;
                }

                var nextPosition = currentPosition + moveDirection;
                if (nextPosition.x > boardSize.x || nextPosition.y > boardSize.y)
                {
                    onFail.Invoke();
                }

                Vector2 targetPosition = GetPositionFromBoard(nextPosition);

                Sequence sequence = DOTween.Sequence();

                sequence.Append(playerRectTransform.DOMove(targetPosition, moveTime));
                if (currentRotation != targetRotation)
                {
                    sequence.Join(playerRectTransform.DORotateQuaternion(targetRotation, moveTime / 2));
                }

                sequence.OnComplete(() =>
                {
                    MoveToNextStep(selectTypes, nextPosition, targetRotation, currentStep + 1, moveTime, onFail);
                });
            }
            else
            {
                playerSkeleton.AnimationState.SetAnimation(0, idleAnimation, true);
            }
        }

        private Vector2 GetPositionFromBoard(Vector2 position)
        {
            int index = (int)((position.y - 1) * boardSize.x + (position.x - 1));
            return groundPosition[index].position;
        }

        #endregion
    }
}
using System.Collections;
using System.Collections.Generic;
using GameScene.Component;
using JetBrains.Annotations;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace GameScene
{
    public class ClickDragController : GameController
    {
        [Header("Reference Controller")]
        [SerializeField] protected GameView view;
        [SerializeField] protected GameModel model;
        [SerializeField] protected PadSelectController padSelectController;
        [SerializeField] protected BoardController boardController;
        [SerializeField] protected Button playButton;
        [Header("Test Param")]
        [SerializeField] protected List<SelectType> generateList;
        [SerializeField] protected List<Vector2> boardMap;
        // System
        protected readonly Dictionary<Vector2, Transform> targetReferences = new();
        protected readonly Dictionary<Vector2, bool> targetChecker = new();
        protected Vector2 currentPlayerPosition;
        protected Target target;
        protected bool valid;
        // Comment form here if finish
        [Header("Remove soon")]
        [SerializeField] protected RectTransform deleteZone;
        [SerializeField] protected RectTransform selectedZone;
        // FOR CONTROL SELECTOR
        protected readonly List<InteractionItem> storeSelected = new();
        protected readonly List<Vector2> storedPosition = new();
        protected bool isDelete;
        protected const float OffSet = 0.2f;
        [CanBeNull] protected InteractionItem selectedObject;

        #region Perform action

        protected void CreateTarget()
        {
            foreach (var position in targetPosition)
            {
                target = Instantiate(model.Resource.TargetModel).GetComponent<Target>();
                target.Init(model.CandySprites[Random.Range(0, model.CandySprites.Count)]);
                boardController.PlaceObjectToBoard(target.GetComponent<Transform>(), position);
                targetChecker.Add(position, false);
                targetReferences.Add(position, target.transform);
            }
        }

        protected IEnumerator StartPlayerMove()
        {
            view.ActiveSavePanel();
            valid = true;
            var controlPart = padSelectController.GetControlPart();
            foreach (var item in controlPart)
            {
                padSelectController.SetDisplayPart(item, true);
                yield return HandleAction(item);
                padSelectController.SetDisplayPart(item, false);
                if (!valid)
                {
                    break;
                }
            }

            view.ActiveSavePanel(false);
            if (WinChecker() && valid)
            {
                ShowWinPopup(700);
                // win
            }
            else
            {
                ResetGame();
            }
        }

        protected IEnumerator HandleAction(InteractionItem direction)
        {
            var isEat = false;
            var targetMove = currentPlayerPosition;
            switch (direction.SelectType)
            {
                case SelectType.Up:
                    targetMove += Vector2.up;
                    break;
                case SelectType.Down:
                    targetMove += Vector2.down;
                    break;
                case SelectType.Left:
                    targetMove += Vector2.left;
                    break;
                case SelectType.Right:
                    targetMove += Vector2.right;
                    break;
                case SelectType.Collect:
                    isEat = true;
                    break;
                case SelectType.Loop:
                default:
                    playerController.PlayAnimationIdle();
                    yield break;
            }

            if (targetMove != currentPlayerPosition)
            {
                currentPlayerPosition = targetMove;

                if (IsOutsideBoard(targetMove))
                {
                    // Reset game cuz it fail
                    playerController.PlayAnimationIdle();
                    yield return new WaitForSeconds(1f);
                    valid = false;
                    yield break;
                }

                yield return playerController.MovePlayer(boardController.GetPositionFromBoard(targetMove),
                    model.PlayerMoveTime);
            }

            if (isEat)
            {
                var tracker = playerController.PlayAnimationEat();
                if (targetChecker.ContainsKey(currentPlayerPosition))
                {
                    targetChecker[currentPlayerPosition] = true;
                    targetReferences[currentPlayerPosition].gameObject.SetActive(false);
                }

                yield return new WaitForSpineAnimationComplete(tracker);
                playerController.PlayAnimationIdle();
            }
        }

        protected void ResetGame()
        {
            padSelectController.Reset();

            // Clear win condition and re-active target
            foreach (var position in targetReferences.Keys)
            {
                targetReferences[position].gameObject.SetActive(true);
                targetChecker[position] = false;
            }

            // Reset player position
            currentPlayerPosition = basePlayerPosition;

            playerController.RotatePlayer(
                targetPosition[0].x >= basePlayerPosition.x
                , 0.1f);
            playerController.PlayAnimationIdle();

            // set player position
            boardController.PlaceObjectToBoard(playerController.transform, basePlayerPosition);
        }

        private bool WinChecker()
        {
            foreach (var value in targetChecker.Values)
            {
                if (!value) // any not get
                {
                    return false;
                }
            }

            return true;
        }

        #endregion

        #region Calulate func

        protected virtual bool IsOutsideBoard(Vector2 checkPosition)
        {
            return checkPosition.x > boardSize.x || checkPosition.y > boardSize.y ||
                   checkPosition.x <= 0 || checkPosition.y <= 0;
        }

        #endregion
    }
}
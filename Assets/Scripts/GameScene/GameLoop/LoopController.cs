using System.Collections;
using System.Collections.Generic;
using GameScene.Component;
using GameScene.Component.SelectControl;
using Services;
using Spine.Unity;
using UnityEngine;
using Utilities;

namespace GameScene.GameLoop
{
    public class LoopController : ClickDragController
    {
        [Header("Reference model")]
        [SerializeField] private LoopView view;
        [SerializeField] private LoopModel model;
        [SerializeField] private PadSelectController padSelectController;
        [SerializeField] private BoardController boardController;

        private void Start()
        {
            gameMode = GameMode.Loop;

            playButton.onClick.AddListener(OnClickPlay);
            padSelectController.CreateSelector(generateList, model.Resource);
            boardController.CreateBoard(new Vector2(8, 6), model.Resource.BoardCellModel);
            playerController = Instantiate(model.PlayerModel).GetComponent<PlayerController>();
            // Init player model
            currentPlayerPosition = basePlayerPosition;
            boardController.PlaceObjectToBoard(playerController.transform, basePlayerPosition);

            CreateTarget();
        }

        private void Update()
        {
            if (Input.GetMouseButtonUp(0))
            {
                padSelectController.HandleMouseUp();
            }

            padSelectController.HandleMouseMoveSelected();
        }

        #region Game Flow

        private IEnumerator StartPlayerMove()
        {
            view.ActiveSavePanel();
            var actionList = ConvertToAction();
            foreach (var action in actionList)
            {
                var item = action;
                view.SetParentSelectedToMove(item.transform);
                item.ActiveEffect();
                yield return HandleAction(item);
                item.ActiveEffect(false);
                view.SetParentSelected(item.transform);
            }

            view.ActiveSavePanel(false);
            if (WinChecker())
            {
                ShowWinPopup(800);
                // win
            }
            else
            {
                ResetGame();
            }
        }

        private IEnumerator HandleAction(InteractionItem action)
        {
            var isMove = true;
            var targetMove = currentPlayerPosition;
            switch (action.SelectType)
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
                    isMove = false;
                    break;
            }

            if (isMove)
            {
                currentPlayerPosition = targetMove;

                if (IsOutsideBoard(targetMove))
                {
                    // Reset game cuz it fail
                    playerController.PlayAnimationIdle();
                    yield return new WaitForSeconds(1f);
                    ResetGame();
                    yield break;
                }

                targetMove = view.GetPositionFromBoard(targetMove);
                yield return MovePlayer(targetMove, model.PlayerMoveTime);
            }
            else
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

        private void ResetGame()
        {
            // Clear all things selected
            foreach (var selector in storeSelected)
            {
                if (selector.SelectType == SelectType.Loop)
                {
                    var looper = (Extensional)selector;

                    foreach (var itemLooped in looper.StoreSelected)
                    {
                        SimplePool.Despawn(itemLooped.gameObject);
                    }

                    looper.StoreSelected.Clear();
                }

                SimplePool.Despawn(selector.gameObject);
            }

            storeSelected.Clear();

            // Clear win condition
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
            view.PlaceObjectToBoard(playerController.transform, basePlayerPosition);
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

        private List<InteractionItem> ConvertToAction()
        {
            var result = new List<InteractionItem>();
            foreach (var item in storeSelected)
            {
                if (item.SelectType == SelectType.Loop)
                {
                    var looper = (Extensional)item;
                    for (int i = 0; i < looper.LoopCount; i++)
                    {
                        foreach (var itemLooped in looper.StoreSelected)
                        {
                            result.Add(itemLooped);
                        }
                    }
                }
                else
                {
                    result.Add(item);
                }
            }

            return result;
        }

        #endregion

        #region Initialized

        private void CreateTarget()
        {
            foreach (var position in targetPosition)
            {
                target = Instantiate(model.TargetPrefab).GetComponent<Target>();
                target.Init(model.CandySprites[Random.Range(0, model.CandySprites.Count)]);
                boardController.PlaceObjectToBoard(target.GetComponent<Transform>(), position);
                targetChecker.Add(position, false);
                targetReferences.Add(position, target.transform);
            }
        }

        #endregion

        #region CALL BACK

        // Start Moving
        private void OnClickPlay()
        {
            StartCoroutine(StartPlayerMove());
        }

        #endregion

        #region Calulate func

        private bool IsOutsideBoard(Vector2 checkPosition)
        {
            if (boardMap.Contains(checkPosition))
            {
                return false;
            }

            if (targetPosition.Contains(checkPosition))
            {
                return false;
            }

            if (basePlayerPosition == checkPosition)
            {
                return false;
            }

            return true;
        }

        #endregion
    }
}
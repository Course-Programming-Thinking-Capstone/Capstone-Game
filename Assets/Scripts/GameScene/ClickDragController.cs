using System.Collections;
using System.Collections.Generic;
using GameScene.Component;
using Services;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace GameScene
{
    public class ClickDragController : GameController
    {
        [Header("Reference Controller")]
        [SerializeField] protected GameView view;
        [SerializeField] protected GameModel model;
        [SerializeField] protected PadSelectController padSelectController;
        [SerializeField] protected Button playButton;
        [Header("Default Param")]
        [SerializeField] protected List<SelectType> generateList;
        // System
        private readonly Dictionary<Vector2, Transform> targetReferences = new();
        private readonly Dictionary<Vector2, bool> targetChecker = new();
        private readonly Dictionary<Vector2, bool> conditionChecker = new();
        protected Vector2 currentPlayerPosition;
        private bool valid;

        #region Perform action

        protected void CreateBlockers()
        {
            foreach (var position in rockMap)
            {
                var target = Instantiate(model.Resource.BlockPrefab);
                boardController.PlaceObjectToBoard(target.GetComponent<Transform>(), position);
            }
        }

        protected void CreateTarget()
        {
            var conditionResource = Resources.Load<Sprite>("Fruits/T_fruit_unknow");
            if (gameMode == GameMode.Condition)
            {
                foreach (var position in targetPosition)
                {
                    var target = Instantiate(model.Resource.TargetModel).GetComponent<Target>();

                    boardController.PlaceObjectToBoard(target.GetComponent<Transform>(), position);

                    targetChecker.Add(position, false);
                    conditionChecker.Add(position, Random.Range(0, 11) < 6); // true -> need condition
                    if (conditionChecker[position])
                    {
                        target.Init(conditionResource);
                    }
                    else
                    {
                        target.Init(model.CandySprites[Random.Range(0, model.CandySprites.Count)]);
                    }

                    targetReferences.Add(position, target.transform);
                }
            }
            else
            {
                foreach (var position in targetPosition)
                {
                    var target = Instantiate(model.Resource.TargetModel).GetComponent<Target>();
                    target.Init(model.CandySprites[Random.Range(0, model.CandySprites.Count)]);
                    boardController.PlaceObjectToBoard(target.GetComponent<Transform>(), position);
                    targetChecker.Add(position, false);
                    targetReferences.Add(position, target.transform);
                }
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
                ShowWinPopup();
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
                case SelectType.Condition:
                    isEat = true;
                    break;
                default:
                    playerController.PlayAnimationIdle();
                    yield break;
            }

            if (targetMove != currentPlayerPosition)
            {
                currentPlayerPosition = targetMove;

                if (IsOutsideBoard(targetMove) || rockMap.Contains(targetMove))
                {
                    // Reset game cuz it fail
                    playerController.PlayAnimationIdle();
                    yield return new WaitForSeconds(1f);
                    valid = false;
                    yield break;
                }

                // Move Player
                yield return playerController.MovePlayer(boardController.GetPositionFromBoard(targetMove),
                    model.PlayerMoveTime);
                boardController.SetGroundAnimation(targetMove, true);
                yield return new WaitForSeconds(0.2f); // Delay
                boardController.SetGroundAnimation(targetMove, false);
            }

            if (isEat)
            {
                var tracker = playerController.PlayAnimationEat();
                if (direction.SelectType == SelectType.Condition)
                {
                    yield return new WaitForSpineAnimationComplete(tracker);
                    if (targetChecker.ContainsKey(currentPlayerPosition)) // candy? -> eat
                    {
                        targetChecker[currentPlayerPosition] = true;
                        targetReferences[currentPlayerPosition].gameObject.SetActive(false);
                    }

                    playerController.PlayAnimationIdle();
                }
                else
                {
                    yield return new WaitForSpineAnimationComplete(tracker);
                    if (targetChecker.ContainsKey(currentPlayerPosition))
                    {
                        if (conditionChecker.ContainsKey(currentPlayerPosition) &&
                            conditionChecker[currentPlayerPosition]) // need condition!
                        {
                            // Reset game cuz you eat something wrong, hohoho
                            valid = false;
                            targetReferences[currentPlayerPosition].gameObject.SetActive(false);
                            playerController.PlayAnimationFail();
                            yield return new WaitForSeconds(1f);
                            yield break;
                        }

                        // normal fruits
                        targetChecker[currentPlayerPosition] = true;
                        targetReferences[currentPlayerPosition].gameObject.SetActive(false);
                    }

                    playerController.PlayAnimationIdle();
                }
            }
        }

        private void ResetGame()
        {
            var param = PopupHelpers.PassParamPopup();
            param.SaveObject(ParamType.ModeGame, gameMode);
            param.SaveObject(ParamType.LevelIndex, levelIndex);

            PopupHelpers.Show(Constants.FailPopup);
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
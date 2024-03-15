using System.Collections;
using GameScene.Component;
using Services;
using Spine.Unity;
using UnityEngine;

namespace GameScene.GameSequence
{
    public class SequenceController : ClickDragController
    {
        [Header("Reference model")]
        [SerializeField] private SequenceView view;
        [SerializeField] private SequenceModel model;
        [SerializeField] private PadSelectController padSelectController;
        [SerializeField] private BoardController boardController;

        #region INITIALIZE

        private void Start()
        {
            gameMode = GameMode.Sequence;
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

        #region Perform action

        private bool valid;

        private IEnumerator StartPlayerMove()
        {
            view.ActiveSavePanel();
            valid = true;
            var controlPart = padSelectController.GetControlPart();
            foreach (var item in controlPart)
            {
                view.SetParentSelectedToMove(item.transform);
                item.ActiveEffect();
                yield return HandleAction(item);
                item.ActiveEffect(false);
                view.SetParentSelected(item.transform);
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

        private IEnumerator HandleAction(InteractionItem direction)
        {
            var isEat = false;
            var isBreak = false;
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
                default:
                    isBreak = true;
                    break;
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

            if (isBreak)
            {
                playerController.PlayAnimationIdle();
                yield return new WaitForSeconds(1f);
            }
        }

        private void ResetGame()
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

        private bool IsOutsideBoard(Vector2 checkPosition)
        {
            return checkPosition.x > boardSize.x || checkPosition.y > boardSize.y ||
                   checkPosition.x <= 0 || checkPosition.y <= 0;
        }

        #endregion

        #region CALL BACK

        // Start Moving
        private void OnClickPlay()
        {
            StartCoroutine(StartPlayerMove());
        }

        #endregion
    }
}
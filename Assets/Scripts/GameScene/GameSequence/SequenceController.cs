using System.Collections;
using GameScene.Component;
using Services;
using Spine.Unity;
using UnityEngine;
using Utilities;

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
            padSelectController.CreateSelector();
            boardController.CreateBoard(new Vector2(8, 6), model.CellBoardPrefab);
            CreateTarget();
            CreatePlayer();
        }

        private void Update()
        {
            if (Input.GetMouseButtonUp(0))
            {
                padSelectController.HandleMouseUp();
            }

            padSelectController.HandleMouseMoveSelected();
        }

        private void CreatePlayer()
        {
            playerControl = Instantiate(model.PlayerModel).GetComponent<Player>();
            currentPlayerPosition = playerPosition;
            boardController.PlaceObjectToBoard(playerControl.transform, playerPosition);
        }

        private void CreateTarget()
        {
            foreach (var position in targetPosition)
            {
                candy = Instantiate(model.TargetPrefab).GetComponent<Candy>();
                candy.Init(model.CandySprites[Random.Range(0, model.CandySprites.Count)]);
                boardController.PlaceObjectToBoard(candy.GetComponent<Transform>(), position);
                targetChecker.Add(position, false);
                targetReferences.Add(position, candy.transform);
            }
        }

        #endregion

        #region Perform action

        private IEnumerator StartPlayerMove()
        {
            view.ActiveSavePanel();
            foreach (var item in storeSelected)
            {
                view.SetParentSelectedToMove(item.transform);
                item.ActiveEffect();
                yield return HandleAction(item);
                item.ActiveEffect(false);
                view.SetParentSelected(item.transform);
            }

            view.ActiveSavePanel(false);
            if (WinChecker())
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
                    playerControl.PlayAnimationIdle();
                    yield return new WaitForSeconds(1f);
                    ResetGame();
                    yield break;
                }

                targetMove = view.GetPositionFromBoard(targetMove);
                yield return MovePlayer(targetMove, model.PlayerMoveTime);
            }

            if (isEat)
            {
                var tracker = playerControl.PlayAnimationEat();

                if (targetChecker.ContainsKey(currentPlayerPosition))
                {
                    targetChecker[currentPlayerPosition] = true;
                    targetReferences[currentPlayerPosition].gameObject.SetActive(false);
                }

                yield return new WaitForSpineAnimationComplete(tracker);
                playerControl.PlayAnimationIdle();
            }

            if (isBreak)
            {
                playerControl.PlayAnimationIdle();
                yield return new WaitForSeconds(1f);
            }
        }

        private void ResetGame()
        {
            // Clear all things selected
            foreach (var selector in storeSelected)
            {
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
            currentPlayerPosition = playerPosition;
            playerControl.RotatePlayer(
                targetPosition[0].x >= playerPosition.x
                , 0.1f);
            playerControl.PlayAnimationIdle();
            boardController.PlaceObjectToBoard(playerControl.transform, playerPosition);
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
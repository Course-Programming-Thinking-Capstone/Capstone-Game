using GameScene.Component;
using Services;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameScene.GameLoop
{
    public class LoopController : ClickDragController
    {
        private async void Start()
        {
            gameMode = GameMode.Loop;
            if (!await LoadData())
            {
                SceneManager.LoadScene(Constants.MainMenu);
                return;
            }
            
            view.SetDetail(gameMode + " mode: " + " Level " + levelIndex);
            playButton.onClick.AddListener(OnClickPlay);
            padSelectController.CreateSelector(generateList, model.Resource);
            boardController.CreateBoard(new Vector2(8, 6), model.Resource.BoardCellModel);

            if (!boardMap.Contains(basePlayerPosition))
            {
                boardMap.Add(basePlayerPosition);
            }

            foreach (var tg in targetPosition)
            {
                if (!boardMap.Contains(tg))
                {
                    boardMap.Add(tg);
                }
            }

            boardController.ActiveSpecificBoard(boardMap);
            playerController = Instantiate(model.PlayerModel).GetComponent<PlayerController>();
            if (targetPosition[0].x > basePlayerPosition.x)
            {
                playerController.RotatePlayer(true, 0.1f);
            }
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

        #region CALL BACK

        // Start Moving
        private void OnClickPlay()
        {
            StartCoroutine(StartPlayerMove());
        }

        #endregion

        #region Calulate func

        protected override bool IsOutsideBoard(Vector2 checkPosition)
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
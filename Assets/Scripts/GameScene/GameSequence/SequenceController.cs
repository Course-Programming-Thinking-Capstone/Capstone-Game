using GameScene.Component;
using Services;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameScene.GameSequence
{
    public class SequenceController : ClickDragController
    {
        #region INITIALIZE

        private async void Start()
        {
            gameMode = GameMode.Sequence;
            if (!await LoadData())
            {
                SceneManager.LoadScene(Constants.MainMenu);
                return;
            }

            view.SetDetail(gameMode + " mode: " + " Level " + levelIndex);

            playButton.onClick.AddListener(OnClickPlay);
            padSelectController.CreateSelector(generateList, model.Resource);
            boardController.CreateBoard(new Vector2(8, 6), model.Resource.BoardCellModel);
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
using GameScene.Component;
using Services;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameScene.GameCondition
{
    public class ConditionController : ClickDragController
    {
        private async void Start()
        {
            gameMode = GameMode.Condition;
            if (!await LoadData())
            {
                SceneManager.LoadScene(Constants.MainMenu);
                return;
            }
            audioService.PlayMusic(MusicToPlay.Water);
            view.SetDetail(gameMode + " mode: " + " Level " + (levelIndex + 1));

            playButton.onClick.AddListener(OnClickPlay);
            padSelectController.CreateSelector(generateList, model.Resource);
            boardController.CreateBoard(new Vector2(8, 6), model.Resource.BoardCellModel);
            playerController = Instantiate(model.Resource.PlayerModel).GetComponent<PlayerController>();
            if (targetPosition[0].x > basePlayerPosition.x)
            {
                playerController.RotatePlayer(true, 0.1f);
            }

            // Init player model
            currentPlayerPosition = basePlayerPosition;
            boardController.PlaceObjectToBoard(playerController.transform, basePlayerPosition);
            CreateTarget();
            CreateBlockers();
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
    }
}
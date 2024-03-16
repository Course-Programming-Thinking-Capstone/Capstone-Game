using System.Collections;
using GameScene.Component;
using Services;
using Spine.Unity;
using UnityEngine;

namespace GameScene.GameSequence
{
    public class SequenceController : ClickDragController
    {
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
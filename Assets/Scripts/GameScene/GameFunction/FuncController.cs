using GameScene.Component;
using Services;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameScene.GameFunction
{
    public class FuncController : ClickDragController
    {
        #region INITIALIZE

        private async void Start()
        {
            gameMode = GameMode.Function;
            if (!await LoadData())
            {
                SceneManager.LoadScene(Constants.MainMenu);
                return;
            }
            audioService.PlayMusic(MusicToPlay.Water);
            view.SetDetail(gameMode + " mode: " + " Level " + (levelIndex+1));
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
            playerController = Instantiate(model.Resource.PlayerModel).GetComponent<PlayerController>();
            if (targetPosition[0].x > basePlayerPosition.x)
            {
                playerController.RotatePlayer(true, 0.1f);
            }
            // Init player model
            currentPlayerPosition = basePlayerPosition;
            boardController.PlaceObjectToBoard(playerController.transform, basePlayerPosition);
            CreateTarget();
        }

        #endregion

        private void Update()
        {
            if (Input.GetMouseButtonUp(0))
            {
                padSelectController.HandleMouseUp();
            }

            padSelectController.HandleMouseMoveSelected();
        }

      
        #region Calulate func

        //
        // private int CalculatedCurrentPosition(Vector2 mousePos, List<Vector2> storedList)
        // {
        //     for (int i = 0; i < storedList.Count; i++)
        //     {
        //         if (i == 0 && storedList[i].y - OffSet < mousePos.y) // first item
        //         {
        //             return 0;
        //         }
        //
        //         if (i == storedList.Count - 1) // last item
        //         {
        //             return storedList.Count;
        //         }
        //
        //         if (storedList[i].y + OffSet > mousePos.y
        //             && storedList[i + 1].y - OffSet < mousePos.y)
        //         {
        //             return i + 1;
        //         }
        //     }
        //
        //     return storedList.Count;
        // }
        //
        // private void HandleFuncDisplayCalculate(Vector2 mousePos)
        // {
        //     if (IsPointInRT(mousePos, selectedZone))
        //     {
        //         // view.MakeEmptySpace(
        //         //     storeFuncSelected.Select(o => o.RectTransform).ToList(),
        //         //     CalculatedCurrentPosition(mousePos, storedFuncPosition),
        //         //     selectedObject.RectTransform.sizeDelta.y
        //         // );
        //     }
        //     else
        //     {
        //         view.ReSortItemsSelected(storeSelected.Select(o => o.RectTransform).ToList());
        //     }
        // }
        //
        // private void HandleDisplayCalculate(Vector2 mousePos)
        // {
        //     if (IsPointInRT(mousePos, selectedZone))
        //     {
        //         // view.MakeEmptySpace(
        //         //     storeSelected.Select(o => o.RectTransform).ToList(),
        //         //     CalculatedCurrentPosition(mousePos, storedPosition),
        //         //     selectedObject.RectTransform.sizeDelta.y
        //         // );
        //     }
        //     else
        //     {
        //         view.ReSortItemsSelected(storeSelected.Select(o => o.RectTransform).ToList());
        //     }
        // }
        //
        // private void StoreTempPosition()
        // {
        //     storedPosition.Clear();
        //     storedFuncPosition.Clear();
        //     foreach (var item in storeSelected)
        //     {
        //         storedPosition.Add(item.RectTransform.position);
        //     }
        //
        //     foreach (var item in storeFuncSelected)
        //     {
        //         storedFuncPosition.Add(item.RectTransform.position);
        //     }
        // }

        #endregion

        // Start Moving
        private void OnClickPlay()
        {
            StartCoroutine(StartPlayerMove());
        }
    }
}
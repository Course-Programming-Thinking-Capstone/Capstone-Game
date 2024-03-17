using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameScene.Component;
using GameScene.Component.SelectControl;
using JetBrains.Annotations;
using Services;
using Spine.Unity;
using UnityEngine;
using Utilities;

namespace GameScene.GameFunction
{
    public class FuncController : ClickDragController
    {
        [Header("Reference model")]
        [SerializeField] private Transform funcTransform;
        // FOR CONTROL SELECTOR
        private readonly List<InteractionItem> storeFuncSelected = new();
        private readonly List<Vector2> storedFuncPosition = new();

        #region INITIALIZE

        private void Start()
        {
            gameMode = GameMode.Loop;
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

        // private void HandleMouseMoveSelected()
        // {
        //     var mousePos = Input.mousePosition;
        //     isDelete = IsPointInRT(mousePos, deleteZone);
        //
        //     var func = CheckInsideFunc();
        //     if (func)
        //     {
        //         view.ReSortItemsSelected(storeFuncSelected.Select(o => o.RectTransform).ToList());
        //         selectedObject!.RectTransform.position = mousePos;
        //         HandleFuncDisplayCalculate(mousePos);
        //         return;
        //     }
        //
        //     selectedObject!.RectTransform.position = mousePos;
        //     // check to make space
        //
        //     if (storeSelected.Count == 10)
        //     {
        //         isDelete = true;
        //         return;
        //     }
        //
        //     view.ReSortItemsSelected(storeSelected.Select(o => o.RectTransform).ToList());
        //     HandleDisplayCalculate(mousePos);
        // }
        //
        // private bool CheckInsideFunc()
        // {
        //     if (selectedObject.SelectType == SelectType.Func)
        //     {
        //         return false;
        //     }
        //
        //     if (storeFuncSelected.Count == 5)
        //     {
        //         return false;
        //     }
        //
        //     var startPosition = (selectedObject.transform.position);
        //     startPosition.z = -5;
        //     Ray ray = new Ray(startPosition, Vector3.forward * 100);
        //     if (Physics.Raycast(ray, out var hit))
        //     {
        //         return funcTransform == hit.transform;
        //     }
        //
        //     return false;
        // }

        
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
        // Start Moving
        private void OnClickPlay()
        {
            StartCoroutine(StartPlayerMove());
        }
        
    }
}
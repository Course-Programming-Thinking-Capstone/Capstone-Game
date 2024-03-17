using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameScene.Component;
using GameScene.Component.SelectControl;
using Services;
using UnityEngine;

namespace GameScene.GameCondition
{
    public class ConditionController : ClickDragController
    {
        private void Start()
        {
            gameMode = GameMode.Condition;
            // LoadData();
        }

        private void Update()
        {
        }

        #region Game Flow

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

        private void FixHeightSelected()
        {
            foreach (var selector in storeSelected)
            {
                if (selector.SelectType == SelectType.Condition)
                {
                    var xSelector = (Condition)selector;
                    xSelector.FixHeightLooper(Vector2.zero);
                }
            }

            view.ReSortItemsSelected(storeSelected.Select(o => o.RectTransform).ToList());
        }

        private List<InteractionItem> ConvertToAction()
        {
            var result = new List<InteractionItem>();
            foreach (var item in storeSelected)
            {
                if (item.SelectType == SelectType.Condition)
                {
                    var looper = (Condition)item;

                    foreach (var itemLooped in looper.StoreSelected)
                    {
                        result.Add(itemLooped);
                    }
                }

                result.Add(item);
            }

            return result;
        }

        #endregion

        #region Initialized

        // private void CreateBoard()
        // {
        //     view.InitGroundBoardFakePosition(boardSize, model.GetBlockOffset());
        //     view.PlaceObjectToBoard(Instantiate(model.CellBoardPrefab).transform, basePlayerPosition);
        //
        //     var listBoard = new List<Transform>();
        //
        //     for (int i = 0; i < boardSize.x * boardSize.y; i++)
        //     {
        //         listBoard.Add(Instantiate(model.CellBoardPrefab).transform);
        //     }
        //
        //     view.InitGroundBoard(listBoard, boardSize, model.GetBlockOffset());
        // }
        //
        // private void CreateSelector()
        // {
        //     // Generate objects selector
        //     foreach (var o in generateList)
        //     {
        //         var obj = Instantiate(model.SelectorPrefab);
        //         view.SetParentSelector(obj.transform);
        //         var scriptControl = obj.AddComponent<Basic>();
        //         scriptControl.Init(OnClickedSelector);
        //         scriptControl.SelectType = o;
        //         scriptControl.ChangeRender(model.GetSelector(o));
        //     }
        // }

        private void CreatePlayer()
        {
            // Init player
            playerController = Instantiate(model.PlayerModel).GetComponent<PlayerController>();
            currentPlayerPosition = basePlayerPosition;
            view.PlaceObjectToBoard(playerController.transform, basePlayerPosition);
        }

        private void InitView()
        {
            // Play button
            playButton.onClick.AddListener(OnClickPlay);
        }

        #endregion

        #region CALL BACK

        private void OnClickedSelected(InteractionItem selectedObj)
        {
            // Get object to move
            // not have?
            storeSelected.Remove(selectedObj);
            foreach (var selector in storeSelected)
            {
                if (selector.SelectType == SelectType.Condition)
                {
                    var looper = (Condition)selector;
                    looper.RemoveItem(selectedObj);
                }
            }

            selectedObject = selectedObj;
            view.SetParentSelectedToMove(selectedObject!.transform);
            view.ReSortItemsSelected(storeSelected.Select(o => o.RectTransform).ToList());
            StoreTempPosition();
        }

        // Start Moving
        private void OnClickPlay()
        {
            StartCoroutine(StartPlayerMove());
        }

        #endregion

        #region Calulate func

        private Condition CheckInsideLoop()
        {
            if (selectedObject.SelectType != SelectType.Collect)
            {
                return null;
            }

            var startPosition = (selectedObject.transform.position);
            startPosition.z = -5;
            Ray ray = new Ray(startPosition, Vector3.forward * 100);
            if (Physics.Raycast(ray, out var hit))
            {
                var result = hit.transform.GetComponent<Condition>();
                return result;
            }

            return null;
        }

        private void StoreTempPosition()
        {
            storedPosition.Clear();
            foreach (var item in storeSelected)
            {
                storedPosition.Add(item.RectTransform.position);
            }
        }

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
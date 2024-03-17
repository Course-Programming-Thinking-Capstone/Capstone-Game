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

        private void HandleMouseMoveSelected()
        {
            var mousePos = Input.mousePosition;
            isDelete = IsPointInRT(mousePos, deleteZone);

            var func = CheckInsideFunc();
            if (func)
            {
                view.ReSortItemsSelected(storeFuncSelected.Select(o => o.RectTransform).ToList());
                selectedObject!.RectTransform.position = mousePos;
                HandleFuncDisplayCalculate(mousePos);
                return;
            }

            selectedObject!.RectTransform.position = mousePos;
            // check to make space

            if (storeSelected.Count == 10)
            {
                isDelete = true;
                return;
            }

            view.ReSortItemsSelected(storeSelected.Select(o => o.RectTransform).ToList());
            HandleDisplayCalculate(mousePos);
        }

        private bool CheckInsideFunc()
        {
            if (selectedObject.SelectType == SelectType.Func)
            {
                return false;
            }

            if (storeFuncSelected.Count == 5)
            {
                return false;
            }

            var startPosition = (selectedObject.transform.position);
            startPosition.z = -5;
            Ray ray = new Ray(startPosition, Vector3.forward * 100);
            if (Physics.Raycast(ray, out var hit))
            {
                return funcTransform == hit.transform;
            }

            return false;
        }

        [CanBeNull]
        private InteractionItem activeFunc;

        private IEnumerator StartPlayerMove()
        {
            view.ActiveSavePanel();
            var actionList = ConvertToAction();
            for (int i = 0; i < actionList.Count; i++)
            {
                var actionItem = actionList[i];
                view.SetParentSelectedToMove(actionItem.transform);
                actionItem.ActiveEffect();
                if (actionItem.SelectType == SelectType.Func && activeFunc == null)
                {
                    activeFunc = actionItem;
                    continue;
                }

                if (actionItem.SelectType == SelectType.Func && activeFunc != null)
                {
                    activeFunc.ActiveEffect(false);
                    view.SetParentSelected(activeFunc.transform);
                    activeFunc = actionItem;
                    continue;
                }

                if (activeFunc != null && !storeFuncSelected.Contains(actionItem)) // other type
                {
                    activeFunc.ActiveEffect(false);
                    view.SetParentSelected(activeFunc.transform);
                    activeFunc = null;
                }

                yield return HandleAction(actionItem);
                actionItem.ActiveEffect(false);
                view.SetParentSelected(actionItem.transform);
            }

            view.ActiveSavePanel(false);
            if (WinChecker())
            {
                ShowWinPopup(800);
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
                    ResetGame();
                    yield break;
                }

                targetMove = view.GetPositionFromBoard(targetMove);
                yield return MovePlayer(targetMove, model.PlayerMoveTime);
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
            // Clear all things selected
            foreach (var selector in storeSelected)
            {
                SimplePool.Despawn(selector.gameObject);
            }

            foreach (var selector in storeFuncSelected)
            {
                SimplePool.Despawn(selector.gameObject);
            }

            storeSelected.Clear();
            storeFuncSelected.Clear();
            // Clear win condition
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
            view.PlaceObjectToBoard(playerController.transform, basePlayerPosition);
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

        #region Calulate func

        private int CalculatedCurrentPosition(Vector2 mousePos, List<Vector2> storedList)
        {
            for (int i = 0; i < storedList.Count; i++)
            {
                if (i == 0 && storedList[i].y - OffSet < mousePos.y) // first item
                {
                    return 0;
                }

                if (i == storedList.Count - 1) // last item
                {
                    return storedList.Count;
                }

                if (storedList[i].y + OffSet > mousePos.y
                    && storedList[i + 1].y - OffSet < mousePos.y)
                {
                    return i + 1;
                }
            }

            return storedList.Count;
        }

        private void HandleFuncDisplayCalculate(Vector2 mousePos)
        {
            if (IsPointInRT(mousePos, selectedZone))
            {
                // view.MakeEmptySpace(
                //     storeFuncSelected.Select(o => o.RectTransform).ToList(),
                //     CalculatedCurrentPosition(mousePos, storedFuncPosition),
                //     selectedObject.RectTransform.sizeDelta.y
                // );
            }
            else
            {
                view.ReSortItemsSelected(storeSelected.Select(o => o.RectTransform).ToList());
            }
        }

        private void HandleDisplayCalculate(Vector2 mousePos)
        {
            if (IsPointInRT(mousePos, selectedZone))
            {
                // view.MakeEmptySpace(
                //     storeSelected.Select(o => o.RectTransform).ToList(),
                //     CalculatedCurrentPosition(mousePos, storedPosition),
                //     selectedObject.RectTransform.sizeDelta.y
                // );
            }
            else
            {
                view.ReSortItemsSelected(storeSelected.Select(o => o.RectTransform).ToList());
            }
        }

        private void StoreTempPosition()
        {
            storedPosition.Clear();
            storedFuncPosition.Clear();
            foreach (var item in storeSelected)
            {
                storedPosition.Add(item.RectTransform.position);
            }

            foreach (var item in storeFuncSelected)
            {
                storedFuncPosition.Add(item.RectTransform.position);
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

        // // Event clicked selector
        // private void OnClickedSelector(InteractionItem selectedObj)
        // {
        //     // Generate new selected
        //
        //     var obj = SimplePool.Spawn(model.SelectedPrefab);
        //     Basic selectedScript = obj.GetComponent<Basic>();
        //     selectedScript.Init(OnClickedSelected);
        //     selectedScript.ChangeRender(model.Resource.GetByType(selectedObj.SelectType.)));
        //     selectedScript.SelectType = selectedObj.SelectType;
        //
        //     // Moving handler
        //     selectedObject = selectedScript;
        //     view.SetParentSelectedToMove(selectedObject.transform);
        //     StoreTempPosition();
        // }
        //
        // private void OnClickedSelected(InteractionItem selectedObj)
        // {
        //     // Get object to move
        //     storeSelected.Remove(selectedObj);
        //     storeFuncSelected.Remove(selectedObj);
        //
        //     selectedObject = selectedObj;
        //     view.SetParentSelectedToMove(selectedObject!.transform);
        //     view.ReSortItemsSelected(storeSelected.Select(o => o.RectTransform).ToList());
        //     view.ReSortItemsSelected(storeFuncSelected.Select(o => o.RectTransform).ToList());
        //
        //     StoreTempPosition();
        // }

        // Start Moving
        private void OnClickPlay()
        {
            StartCoroutine(StartPlayerMove());
        }

        private List<InteractionItem> ConvertToAction()
        {
            var result = new List<InteractionItem>();
            foreach (var item in storeSelected)
            {
                result.Add(item);
                if (item.SelectType == SelectType.Func)
                {
                    foreach (var itemInFunc in storeFuncSelected)
                    {
                        result.Add(itemInFunc);
                    }
                }
            }

            return result;
        }
    }
}
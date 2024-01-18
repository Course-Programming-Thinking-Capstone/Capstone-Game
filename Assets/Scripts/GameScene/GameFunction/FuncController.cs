using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameScene.Component;
using GameScene.Component.SelectControl;
using Spine.Unity;
using UnityEngine;

namespace GameScene.GameFunction
{
    public class FuncController : GameController
    {
        [Header("Reference model")]
        [SerializeField] private FuncView view;
        [SerializeField] private FuncModel model;
        [SerializeField] private Transform funcTransform;
        [Header("Demo param")]
        [SerializeField]
        private List<Vector2> boardMap;

        // FOR CONTROL SELECTOR
        private readonly List<Selector> storeSelected = new();
        private readonly List<Selector> storeFuncSelected = new();
        private readonly List<Vector2> storedPosition = new();
        private readonly List<Vector2> storedFuncPosition = new();
        private bool isDelete;
        private const float OffSet = 0.2f;
        private Selector selectedObject;

        // System
        private Candy candy;
        private Vector2 currentPlayerPosition;
        private readonly Dictionary<Vector2, bool> targetChecker = new();
        private readonly Dictionary<Vector2, Transform> targetReferences = new();

        #region INITIALIZE

        private void Start()
        {
            CreateSelector();
            CreateBoard();
            CreateTarget();
            CreatePlayer();
            InitView();
        }

        private void CreateBoard()
        {
            view.InitGroundBoardFakePosition(boardSize, model.GetBlockOffset());
            view.PlaceObjectToBoard(Instantiate(model.CellBoardPrefab).transform, playerPosition);
            foreach (var target in targetPosition)
            {
                view.PlaceObjectToBoard(Instantiate(model.CellBoardPrefab).transform, target);
            }

            foreach (var positionRoad in boardMap)
            {
                var newRoad = Instantiate(model.CellBoardPrefab);
                view.PlaceObjectToBoard(newRoad.transform, positionRoad);
            }
        }

        private void CreateSelector()
        {
            // Generate objects selector
            foreach (var o in generateList)
            {
                var obj = Instantiate(model.SelectorPrefab);
                view.SetParentSelector(obj.transform);
                var scriptControl = obj.AddComponent<Arrow>();
                scriptControl.Init(OnClickedSelector);
                scriptControl.SelectType = o;
                scriptControl.ChangeRender(model.GetSelector(o));
            }
        }

        private void CreatePlayer()
        {
            // Init player
            playerControl = Instantiate(model.PlayerModel).GetComponent<Player>();
            currentPlayerPosition = playerPosition;
            view.PlaceObjectToBoard(playerControl.transform, playerPosition);
        }

        private void CreateTarget()
        {
            // Init Candy
            foreach (var position in targetPosition)
            {
                candy = Instantiate(model.TargetPrefab).GetComponent<Candy>();
                candy.Init(model.CandySprites[Random.Range(0, model.CandySprites.Count)]);
                view.PlaceObjectToBoard(candy.GetComponent<Transform>(), position);
                targetChecker.Add(position, false);
                targetReferences.Add(position, candy.transform);
            }
        }

        private void InitView()
        {
            // Play button
            playButton.onClick.AddListener(OnClickPlay);
        }

        #endregion

        private void Update()
        {
            if (selectedObject)
            {
                if (Input.GetMouseButtonUp(0))
                {
                    HandleMouseUp();
                }
                else
                {
                    HandleMouseMoveSelected();
                }
            }
        }

        private void HandleMouseUp()
        {
            if (isDelete) // in delete zone
            {
                SimplePool.Despawn(selectedObject!.gameObject);
                selectedObject = null;
                isDelete = false;
                return;
            }

            // Valid pos

            var func = CheckInsideFunc();
            if (func)
            {
                if (!storeFuncSelected.Contains(selectedObject))
                {
                    storeFuncSelected.Insert(CalculatedCurrentPosition(Input.mousePosition, storedFuncPosition), selectedObject);
                }

                view.SetParentFuncSelected(selectedObject!.transform);
            }
            else
            {
                if (!storeSelected.Contains(selectedObject))
                {
                    storeSelected.Insert(CalculatedCurrentPosition(Input.mousePosition, storedPosition), selectedObject);
                }

                view.SetParentSelected(selectedObject!.transform);
            }

            view.ReSortItemsSelected(storeSelected.Select(o => o.RectTransform).ToList());
            view.ReSortItemsSelected(storeFuncSelected.Select(o => o.RectTransform).ToList());
            selectedObject = null;
        }

        private void HandleMouseMoveSelected()
        {
            var mousePos = Input.mousePosition;
            isDelete = IsPointInRT(mousePos, deleteZone);
            var func = CheckInsideFunc();
            if (func)
            {
                view.ReSortItemsSelected(storeSelected.Select(o => o.RectTransform).ToList());
                selectedObject!.RectTransform.position = mousePos;
                HandleFuncDisplayCalculate(mousePos);
                return;
            }

            selectedObject!.RectTransform.position = mousePos;
            // check to make space
            HandleDisplayCalculate(mousePos);
        }

        private bool CheckInsideFunc()
        {
            if (selectedObject.SelectType == SelectType.Func)
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

        private IEnumerator StartPlayerMove()
        {
            view.ActiveSavePanel();
            for (int i = 0; i < storeSelected.Count; i++)
            {
                var actionType = storeSelected[i].SelectType;
                yield return HandleAction(actionType);
            }

            view.ActiveSavePanel(false);
            if (WinChecker())
            {
                Debug.Log("You win");
                // win
            }
            else
            {
                ResetGame();
            }
        }

        private IEnumerator HandleAction(SelectType direction)
        {
            var isMove = true;
            var targetMove = currentPlayerPosition;
            switch (direction)
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
                    isMove = false;
                    break;
            }

            if (isMove)
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
            else
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
            view.PlaceObjectToBoard(playerControl.transform, playerPosition);
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
                view.MakeEmptySpace(
                    storeFuncSelected.Select(o => o.RectTransform).ToList(),
                    CalculatedCurrentPosition(mousePos, storedFuncPosition),
                    selectedObject.RectTransform.sizeDelta.y
                );
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
                view.MakeEmptySpace(
                    storeSelected.Select(o => o.RectTransform).ToList(),
                    CalculatedCurrentPosition(mousePos, storedPosition),
                    selectedObject.RectTransform.sizeDelta.y
                );
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
            return checkPosition.x > boardSize.x || checkPosition.y > boardSize.y ||
                   checkPosition.x <= 0 || checkPosition.y <= 0;
        }

        #endregion

        // Event clicked selector
        private void OnClickedSelector(Selector selectedObj)
        {
            // Generate new selected

            var obj = SimplePool.Spawn(model.SelectedPrefab);
            Arrow selectedScript = obj.GetComponent<Arrow>();
            selectedScript.Init(OnClickedSelected);
            selectedScript.ChangeRender(model.GetSelected(selectedObj.SelectType));
            selectedScript.SelectType = selectedObj.SelectType;

            // Moving handler
            selectedObject = selectedScript;
            view.SetParentSelectedToMove(selectedObject.transform);
            StoreTempPosition();
        }

        private void OnClickedSelected(Selector selectedObj)
        {
            // Get object to move
            storeSelected.Remove(selectedObj);
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
    }
}
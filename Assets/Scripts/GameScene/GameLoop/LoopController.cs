using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameScene.Component;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameScene.GameLoop
{
    public class LoopController : GameController
    {
        [Header("Reference model")]
        [SerializeField] private LoopView view;
        [SerializeField] private LoopModel model;

        // FOR CONTROL SELECTOR
        private readonly List<Selector> storeSelector = new();
        private readonly List<Selector> storeSelected = new();
        private readonly List<Vector2> storedPosition = new();
        private bool isDelete;
        private readonly float offSet = 0.2f;
        private Selector selectedObject;

        // System
        private Candy candy;
        private Vector2 currentPlayerPosition;
        private readonly Dictionary<Vector2, bool> targetChecker = new();
        private readonly Dictionary<Vector2, Transform> targetReferences = new();

        private void Start()
        {
            // LoadData();
            CreateSelector();
            CreateBoard();
            CreateTarget();
            CreatePlayer();
            InitView();
        }

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

        #region Game Flow

        private void HandleMouseUp()
        {
            if (isDelete) // in delete zone
            {
                SimplePool.Despawn(selectedObject!.gameObject);
                selectedObject = null;
                isDelete = false;
            }
            else // Valid pos
            {
                if (!storeSelected.Contains(selectedObject))
                {
                    storeSelected.Insert(CalculatedCurrentPosition(Input.mousePosition), selectedObject);
                }

                view.SetParentSelected(selectedObject!.transform);
                view.ReSortItemsSelected(storeSelected.Select(o => o.RectTransform).ToList());
                selectedObject = null;
            }
        }

        private void HandleMouseMoveSelected()
        {
            Vector3 mousePos = Input.mousePosition;
            selectedObject!.RectTransform.position = mousePos;
            // handle if inside delete zone
            isDelete = IsPointInRT(mousePos, deleteZone);

            if (CheckInsideLoop())
            {
                view.ReSortItemsSelected(storeSelected.Select(o => o.RectTransform).ToList());
                return;
            }

            // check to make space
            HandleDisplayCalculate(mousePos);
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

        private IEnumerator HandleAction(SelectType actionType)
        {
            return null;
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

        #endregion

        #region Initialized

        private void CreateBoard()
        {
            var listBoard = new List<Transform>();

            for (int i = 0; i < boardSize.x * boardSize.y; i++)
            {
                listBoard.Add(Instantiate(model.CellBoardPrefab).transform);
            }

            view.InitGroundBoard(listBoard, boardSize, model.GetBlockOffset());
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

                storeSelector.Add(scriptControl);
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

        #region CALL BACK

        // Event clicked selector
        private void OnClickedSelector(Selector selectedObj)
        {
            // Generate new selected

            if (selectedObj.SelectType == SelectType.Loop)
            {
                var objLoop = SimplePool.Spawn(model.LoopPrefab);
                Loop selectedScript = objLoop.GetComponent<Loop>();
                selectedScript.Init(OnClickedSelected);
                selectedScript.SelectType = selectedObj.SelectType;
                // Moving handler
                selectedObject = selectedScript;
                view.SetParentSelectedToMove(selectedObject.transform);
                StoreTempPosition();
            }
            else
            {
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

        #endregion

        #region Calulate func

        private int CalculatedCurrentPosition(Vector2 mousePos)
        {
            for (int i = 0; i < storedPosition.Count; i++)
            {
                if (i == 0 && storedPosition[i].y - offSet < mousePos.y) // first item
                {
                    return 0;
                }

                if (i == storedPosition.Count - 1) // last item
                {
                    return storedPosition.Count;
                }

                if (storedPosition[i].y + offSet > mousePos.y
                    && storedPosition[i + 1].y - offSet < mousePos.y)
                {
                    return i + 1;
                }
            }

            return storedPosition.Count;
        }

        private void HandleDisplayCalculate(Vector2 mousePos)
        {
            if (IsPointInRT(mousePos, selectedZone))
            {
                view.MakeEmptySpace(
                    storeSelected.Select(o => o.RectTransform).ToList(),
                    CalculatedCurrentPosition(mousePos),
                    selectedObject.RectTransform.sizeDelta.y
                );
            }
            else
            {
                view.ReSortItemsSelected(storeSelected.Select(o => o.RectTransform).ToList());
            }
        }

        private bool CheckInsideLoop()
        {
            if (selectedObject.SelectType == SelectType.Loop)
            {
                return false;
            }

            var startPosition = (selectedObject.transform.position);
            startPosition.z = -5;
            Ray ray = new Ray(startPosition, Vector3.forward * 100);
            if (Physics.Raycast(ray, out var hit))
            {
                Debug.Log(hit);
                return hit.transform.TryGetComponent(typeof(Loop), out _);
            }

            return false;
        }

        private bool IsPointInRT(Vector2 point, RectTransform rt)
        {
            // Get the rectangular bounding box of your UI element
            var rect = rt.rect;
            var anchoredPosition = rt.position;
            // Get the left, right, top, and bottom boundaries of the rect
            float leftSide = anchoredPosition.x - rect.width / 2f;
            float rightSide = anchoredPosition.x + rect.width / 2f;
            float topSide = anchoredPosition.y + rect.height / 2f;
            float bottomSide = anchoredPosition.y - rect.height / 2f;

            // Check to see if the point is in the calculated bounds
            if (point.x >= leftSide &&
                point.x <= rightSide &&
                point.y >= bottomSide &&
                point.y <= topSide)
            {
                return true;
            }

            return false;
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
            return checkPosition.x > boardSize.x || checkPosition.y > boardSize.y ||
                   checkPosition.x <= 0 || checkPosition.y <= 0;
        }

        #endregion
    }
}
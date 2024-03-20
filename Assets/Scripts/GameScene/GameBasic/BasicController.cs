using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameScene.Component;
using GameScene.Component.GameBasic;
using Services;
using UnityEngine;

namespace GameScene.GameBasic
{
    public class BasicGameController : GameController
    {
        [Header("Reference model")]
        [SerializeField] private BasicGameView gameView;
        [SerializeField] private GameModel model;
        // System
        private List<GroundRoad> listBoard = new();
        private List<InteractionItem> listSelector = new();

        private InteractionItem selectedObject;
        private List<SelectType> answer = new();

        private async void Start()
        {
            gameMode = GameMode.Basic;
            foreach (var item in boardMap)
            {
                Debug.Log(item);
            }

            await LoadData();
            Validation();
            CalcSolution();
            GenerateGround();
            GenerateSelector();
            GeneratePlayer();
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

        private void HandleMouseUp()
        {
            var hitObj = CheckValidPosition();
            if (hitObj) // Drag valid
            {
                if (hitObj.CurrentDisplay != null) // SWAP
                {
                    var oldPart = hitObj.CurrentDisplay;
                    hitObj.ChangeRender(model.Resource.GetByType(selectedObject.SelectType).SelectedRender,
                        selectedObject);
                    oldPart.gameObject.SetActive(true);
                    selectedObject.gameObject.SetActive(false);
                    gameView.AddRoadToContainer(oldPart.transform);
                }
                else // current is Null? -> Add new
                {
                    hitObj.ChangeRender(model.Resource.GetByType(selectedObject.SelectType).SelectedRender,
                        selectedObject);
                    selectedObject.gameObject.SetActive(false);
                }
            }
            else // Drag not valid
            {
                gameView.AddRoadToContainer(selectedObject.transform);
            }

            selectedObject = null;

            // Checking for play

            foreach (var item in listBoard)
            {
                if (item.CurrentDisplay == null)
                {
                    return;
                }
            }

            StartCoroutine(StartPlayerMove());
        }

        private void HandleMouseMoveSelected()
        {
            Vector3 mousePos = Input.mousePosition;
            selectedObject.RectTransform.position = mousePos;
        }

        private IEnumerator StartPlayerMove()
        {
            gameView.ActiveSavePanel();

            for (int i = 0; i < answer.Count; i++)
            {
                if (answer[i] == listBoard[i].CurrentDisplay.SelectType)
                {
                    // Create a promise for the current animation
                    var targetMove = listBoard[i].transform.position;
                    yield return MovePlayer(targetMove, model.PlayerMoveTime);
                }
                else
                {
                    yield return new WaitForSeconds(1);
                    gameView.ActiveSavePanel(false);
                    ResetGame();
                    yield break;
                }
            }

            yield return MovePlayer(boardController.GetPositionFromBoard(targetPosition[0]), model.PlayerMoveTime);
            ShowWinPopup(800);
            gameView.ActiveSavePanel(false);
        }

        private void ResetGame()
        {
            // player position
            playerController.transform.position = boardController.GetPositionFromBoard(basePlayerPosition);
            playerController.PlayAnimationIdle();
            playerController.RotatePlayer(true, 0.1f);

            // board
            foreach (var item in listBoard)
            {
                item.ChangeRender(null, null);
            }

            // Selector
            gameView.CountLeft = 0;
            gameView.CountRight = 0;
            foreach (var item in listSelector)
            {
                gameView.AddRoadToContainer(item.transform);
                item.gameObject.SetActive(true);
            }
        }

        #region INITIALIZE

        private void GenerateSelector()
        {
            // Selector
            foreach (var item in answer)
            {
                var newObj = Instantiate(model.Resource.RoadSelector);
                gameView.AddRoadToContainer(newObj.transform);
                newObj.transform.localScale = Vector3.one;
                var scriptControl = newObj.GetComponent<Road>();
                scriptControl.Init(OnClickSelector);
                scriptControl.SelectType = item;
                listSelector.Add(scriptControl);
                scriptControl.ChangeRender(model.Resource.GetByType(item).UnSelectRender);
            }
        }

        private void GenerateGround()
        {
            // Ground
            boardController.CreateBoard(new Vector2(8, 6), model.Resource.BoardRoadCell);
            if (!boardMap.Contains(basePlayerPosition))
            {
                boardMap.Add(basePlayerPosition);
            }

            if (!boardMap.Contains(targetPosition[0]))
            {
                boardMap.Add(targetPosition[0]);
            }

            listBoard = boardController.ActiveSpecificBoard<GroundRoad>(boardMap);
            listBoard.Remove(boardController.GetPartAtPosition<GroundRoad>(basePlayerPosition));
            listBoard.Remove(boardController.GetPartAtPosition<GroundRoad>(targetPosition[0]));
            foreach (var controlBoard in listBoard)
            {
                controlBoard.Initialized(OnClickRoad);
                controlBoard.ChangeToQuestionRender();
            }
        }

        private void GeneratePlayer()
        {
            playerController = Instantiate(model.Resource.PlayerModel).GetComponent<PlayerController>();
            boardController.PlaceObjectToBoard(playerController.transform, basePlayerPosition);
        }

        #endregion

        #region CALL BACK

        /// <summary>
        /// Clicked Road in game
        /// </summary>
        /// <param name="arg0"></param>
        private void OnClickRoad(GroundRoad arg0)
        {
            var oldPart = arg0.CurrentDisplay;
            arg0.ChangeToQuestionRender();
            oldPart.gameObject.SetActive(true);
            selectedObject = oldPart;
            gameView.GetRoadToMove(oldPart.transform);
        }

        private void OnClickSelector(InteractionItem road)
        {
            selectedObject = road;
            gameView.GetRoadToMove(selectedObject.transform);
        }

        #endregion

        #region Calculate code

        private bool Validation()
        {
            if (boardMap.Count != boardMap.Distinct().Count())
            {
                Debug.LogError("Duplicate values found in roadPartPositions!");
                return false;
            }

            var sortedPath = SortRoadPartPositions();
            if (sortedPath == null)
            {
                Debug.LogError("Not Connect!");
                return false;
            }

            sortedPath.Remove(targetPosition[0]);
            boardMap = sortedPath;

            return true;
        }

        private List<Vector2> SortRoadPartPositions()
        {
            HashSet<Vector2> visitedNodes = new HashSet<Vector2>();
            Queue<Vector2> queue = new Queue<Vector2>();
            Dictionary<Vector2, Vector2> previousNodes = new Dictionary<Vector2, Vector2>();

            queue.Enqueue(basePlayerPosition);

            while (queue.Count > 0)
            {
                Vector2 currentNode = queue.Dequeue();

                if (currentNode == targetPosition[0])
                {
                    // Reconstruct the path
                    List<Vector2> path = new List<Vector2>();
                    Vector2 current = currentNode;
                    while (current != basePlayerPosition)
                    {
                        path.Add(current);
                        current = previousNodes[current];
                    }

                    path.Reverse(); // Reverse to get the correct order
                    return path;
                }

                visitedNodes.Add(currentNode);

                List<Vector2> neighbors = GetNeighbors(currentNode);

                foreach (Vector2 neighbor in neighbors)
                {
                    if (!visitedNodes.Contains(neighbor) && !queue.Contains(neighbor))
                    {
                        queue.Enqueue(neighbor);
                        previousNodes[neighbor] = currentNode;
                    }
                }
            }

            return null; // No path found
        }

        private void CalcSolution()
        {
            Queue<Vector2> queue = new Queue<Vector2>();
            var result = new List<SelectType>();
            var allPart = new List<Vector2>();
            foreach (var roadPart in boardMap)
            {
                queue.Enqueue(roadPart);
            }

            foreach (var roadPart in boardMap)
            {
                allPart.Add(roadPart);
            }

            allPart.Add(basePlayerPosition);
            allPart.Add(targetPosition[0]);

            while (queue.Count > 0)
            {
                Vector2 currentNode = queue.Dequeue();
                var calcPart = CalcPartType(currentNode, allPart);

                if (calcPart != SelectType.None)
                {
                    result.Add(calcPart);
                }
            }

            answer = result;
        }

        private SelectType CalcPartType(Vector2 node, List<Vector2> roadToCheck)
        {
            if (roadToCheck.Contains(new Vector2(node.x - 1, node.y)) &&
                roadToCheck.Contains(new Vector2(node.x + 1, node.y)))
            {
                return SelectType.RoadHorizontal;
            }

            if (roadToCheck.Contains(new Vector2(node.x, node.y - 1)) &&
                roadToCheck.Contains(new Vector2(node.x, node.y + 1)))
            {
                return SelectType.RoadVertical;
            }

            if (roadToCheck.Contains(new Vector2(node.x - 1, node.y)) &&
                roadToCheck.Contains(new Vector2(node.x, node.y + 1)))
            {
                return SelectType.RoadTurn3;
            }

            if (roadToCheck.Contains(new Vector2(node.x, node.y + 1)) &&
                roadToCheck.Contains(new Vector2(node.x + 1, node.y)))
            {
                return SelectType.RoadTurn4;
            }

            if (roadToCheck.Contains(new Vector2(node.x + 1, node.y)) &&
                roadToCheck.Contains(new Vector2(node.x, node.y - 1)))
            {
                return SelectType.RoadTurn1;
            }

            if (roadToCheck.Contains(new Vector2(node.x, node.y - 1)) &&
                roadToCheck.Contains(new Vector2(node.x - 1, node.y)))
            {
                return SelectType.RoadTurn2;
            }

            if (roadToCheck.Contains(new Vector2(node.x - 1, node.y)) ||
                roadToCheck.Contains(new Vector2(node.x + 1, node.y)))
            {
                return SelectType.RoadHorizontal;
            }

            if (roadToCheck.Contains(new Vector2(node.x, node.y - 1)) ||
                roadToCheck.Contains(new Vector2(node.x, node.y + 1)))
            {
                return SelectType.RoadVertical;
            }

            return SelectType.None;
        }

        private List<Vector2> GetNeighbors(Vector2 node)
        {
            List<Vector2> neighbors = new List<Vector2>();

            Vector2[] neighborCoordinates =
            {
                new(node.x + 1, node.y),
                new(node.x - 1, node.y),
                new(node.x, node.y + 1),
                new(node.x, node.y - 1)
            };

            foreach (Vector2 neighborCoord in neighborCoordinates)
            {
                if (boardMap.Contains(neighborCoord))
                {
                    neighbors.Add(neighborCoord);
                }
            }

            if (neighborCoordinates.Contains(targetPosition[0]))
            {
                neighbors.Add(targetPosition[0]);
            }

            return neighbors;
        }

        private GroundRoad CheckValidPosition()
        {
            var startPosition = Camera.main!.ScreenToWorldPoint(selectedObject.transform.position);
            Ray ray = new Ray(startPosition, Vector3.forward);
            if (Physics.Raycast(ray, out var hit))
            {
                Transform hitTransform = hit.transform;
                return listBoard.FirstOrDefault(o => o.transform == hitTransform);
            }

            return null;
        }

        #endregion
    }
}
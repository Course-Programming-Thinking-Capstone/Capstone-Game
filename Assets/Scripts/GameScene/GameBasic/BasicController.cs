using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameScene.Component;
using GameScene.Component.GameBasic;
using UnityEngine;

namespace GameScene.GameBasic
{
    public class BasicGameController : GameController
    {
        [Header("Reference model")]
        [SerializeField] private BasicView view;
        [SerializeField] private BasicModel model;
        // System
        private readonly List<GroundRoad> listBoard = new();
        private readonly List<Selector> listSelector = new();
        private readonly Vector2 boardSize = new(8, 6);
        private Vector2 startPosGame;
        private Vector2 endPosGame;
        private Selector selectedObject;
        private List<SelectType> answer = new();
        [Header("Demo param")]
        [SerializeField] [Tooltip("Max 8x6")]
        private List<Vector2> roadPartPositions;

        private void Start()
        {
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
                    hitObj.ChangeRender(model.GetSprite(selectedObject.SelectType), selectedObject);
                    oldPart.gameObject.SetActive(true);
                    selectedObject.gameObject.SetActive(false);
                    view.AddRoadToContainer(oldPart.transform);
                }
                else // Null -> Add new
                {
                    hitObj.ChangeRender(model.GetSprite(selectedObject.SelectType), selectedObject);
                    selectedObject.gameObject.SetActive(false);
                }
            }
            else // Drag not valid
            {
                view.AddRoadToContainer(selectedObject.transform);
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
            view.ActiveSavePanel();

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
                    view.ActiveSavePanel(false);
                    ResetGame();
                    yield break;
                }
            }

            yield return MovePlayer(endPosGame, model.PlayerMoveTime);
            playerControl.PlayAnimationEat();
            view.ActiveSavePanel(false);
        }

        private void ResetGame()
        {
            // player position
            playerControl.transform.position = startPosGame;
            playerControl.PlayAnimationIdle();
            playerControl.RotatePlayer(true, 0.1f);

            // board
            foreach (var item in listBoard)
            {
                item.ChangeRender(model.GetSprite(SelectType.None), null);
            }

            // Selector
            view.CountLeft = 0;
            view.CountRight = 0;
            foreach (var item in listSelector)
            {
                view.AddRoadToContainer(item.transform);
                item.gameObject.SetActive(true);
            }
        }

        #region INITIALIZE

        private void GenerateSelector()
        {
            // Selector
            foreach (var item in answer)
            {
                var newObj = Instantiate(model.RoadToSelect);
                view.AddRoadToContainer(newObj.transform);
                var scriptControl = newObj.GetComponent<Road>();
                scriptControl.Init(OnClickSelector);
                scriptControl.SelectType = item;
                listSelector.Add(scriptControl);
                scriptControl.ChangeRender(model.GetSprite(item));
            }
        }

        private void GenerateGround()
        {
            // Ground
            view.InitGroundBoardFakePosition(boardSize, model.GetBlockOffset());

            startPosGame = view.PlaceGround(Instantiate(model.RoadGroundPrefab).transform, playerPosition);
            endPosGame = view.PlaceGround(Instantiate(model.RoadGroundPrefab).transform, targetPosition);

            foreach (var positionRoad in roadPartPositions)
            {
                var newRoad = Instantiate(model.RoadGroundPrefab);
                var scriptControl = newRoad.GetComponent<GroundRoad>();
                scriptControl.Initialized(OnClickRoad);
                scriptControl.ChangeRender(model.GetSprite(SelectType.None), null);
                listBoard.Add(scriptControl);
                view.PlaceGround(newRoad.transform, positionRoad);
            }
        }

        private void GeneratePlayer()
        {
            // Init player
            playerControl = Instantiate(model.PlayerModel).GetComponent<Player>();
            view.PlacePlayer(playerControl.transform, playerPosition);
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
            arg0.ChangeRender(model.GetSprite(SelectType.None), null);
            oldPart.gameObject.SetActive(true);
            selectedObject = oldPart;
            view.GetRoadToMove(oldPart.transform);
        }

        private void OnClickSelector(Selector road)
        {
            selectedObject = road;
            view.GetRoadToMove(selectedObject.transform);
        }

        #endregion

        #region Calculate code

        private bool Validation()
        {
            if (roadPartPositions.Count != roadPartPositions.Distinct().Count())
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

            sortedPath.Remove(targetPosition);
            roadPartPositions = sortedPath;

            return true;
        }

        private List<Vector2> SortRoadPartPositions()
        {
            HashSet<Vector2> visitedNodes = new HashSet<Vector2>();
            Queue<Vector2> queue = new Queue<Vector2>();
            Dictionary<Vector2, Vector2> previousNodes = new Dictionary<Vector2, Vector2>();

            queue.Enqueue(playerPosition);

            while (queue.Count > 0)
            {
                Vector2 currentNode = queue.Dequeue();

                if (currentNode == targetPosition)
                {
                    // Reconstruct the path
                    List<Vector2> path = new List<Vector2>();
                    Vector2 current = currentNode;
                    while (current != playerPosition)
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
            foreach (var roadPart in roadPartPositions)
            {
                queue.Enqueue(roadPart);
            }

            foreach (var roadPart in roadPartPositions)
            {
                allPart.Add(roadPart);
            }

            allPart.Add(playerPosition);
            allPart.Add(targetPosition);

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
                if (roadPartPositions.Contains(neighborCoord))
                {
                    neighbors.Add(neighborCoord);
                }
            }

            if (neighborCoordinates.Contains(targetPosition))
            {
                neighbors.Add(targetPosition);
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
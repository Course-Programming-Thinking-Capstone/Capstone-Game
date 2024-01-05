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
        private GameObject player;
        private List<GroundRoad> listBoard = new();
        private readonly Vector2 boardSize = new(8, 6);
        private Selector selectedObject;
        [Header("Demo param")]
        // Demo, parameter need
        private readonly List<SelectType> original = new()
        {
            SelectType.RoadHorizontal,
            SelectType.RoadHorizontal,
            SelectType.RoadVertical,
            SelectType.RoadTurn1,
            SelectType.RoadTurn2,
            SelectType.RoadTurn3,
            SelectType.RoadTurn4,
        };

        [SerializeField] [Tooltip("Max 8x6")]
        private List<Vector2> roadPartPositions;

        private void Start()
        {
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
            else // Drag not valis
            {
                view.AddRoadToContainer(selectedObject.transform);
            }

            selectedObject = null;
        }

        private void HandleMouseMoveSelected()
        {
            Vector3 mousePos = Input.mousePosition;
            selectedObject.RectTransform.position = mousePos;
        }

        #region INITIALIZE

        private void GenerateSelector()
        {
            // Selector
            foreach (var item in original)
            {
                var newObj = Instantiate(model.RoadToSelect);
                view.AddRoadToContainer(newObj.transform);
                var scriptControl = newObj.GetComponent<Road>();
                scriptControl.Init(OnClickSelector);
                scriptControl.SelectType = item;
                scriptControl.ChangeRender(model.GetSprite(item));
            }
        }

        private void GenerateGround()
        {
            // Ground
            view.InitGroundBoardFakePosition(boardSize, model.GetBlockOffset());

            view.PlaceGround(Instantiate(model.RoadGroundPrefab).transform, playerPosition);
            view.PlaceGround(Instantiate(model.RoadGroundPrefab).transform, targetPosition);

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
            player = Instantiate(model.PlayerModel);
            view.PlacePlayer(player.GetComponent<Transform>(), playerPosition);
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

        private GroundRoad CheckValidPosition()
        {
            var startPosition = Camera.main.ScreenToWorldPoint(selectedObject.transform.position);
            Ray ray = new Ray(startPosition, Vector3.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Transform hitTransform = hit.transform;
                return listBoard.FirstOrDefault(o => o.transform == hitTransform);
            }

            return null;
        }
    }
}
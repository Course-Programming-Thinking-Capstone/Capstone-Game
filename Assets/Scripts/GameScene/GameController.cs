using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameScene.Component;
using JetBrains.Annotations;
using Services;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace GameScene
{
    public class GameController : MonoBehaviour
    {
        [Header("Reference model")]
        [SerializeField] private GameView view;
        [SerializeField] private GameModel model;

        [Header("Reference object game")]
        [SerializeField] private RectTransform deleteZone;
        [SerializeField] private RectTransform selectedZone;
        [SerializeField] private Button playButton;

        [Header("Testing only")]
        [SerializeField] private List<SelectType> generateList;
        [SerializeField] private Vector2 startPosition;
        [SerializeField] private Vector2 targetPosition;
        [SerializeField] private Vector2 boardSize;

        // SYSTEM
        private readonly List<Selector> storeSelector = new();
        private readonly List<Selector> storeSelected = new();
        private readonly List<Vector2> storedPosition = new();
        private bool isDelete;
        private GameObject player;
        private Candy candy;
        private Vector2 playerPosition;
        private float offSet = 0.2f;
        [CanBeNull] private Selector selectedObject;

        #region INITIALIZE

        private void Awake()
        {
            // Load services
            if (GameObject.FindGameObjectWithTag(Constants.ServicesTag) != null)
            {
                var services = GameObject.FindGameObjectWithTag(Constants.ServicesTag).GetComponent<GameServices>();
            }
            else
            {
                //  SceneManager.LoadScene(Constants.EntryScene);
            }
        }

        private void Start()
        {
            InitScene();
        }

        private void InitScene()
        {
            // Generate objects selector
            foreach (var o in generateList)
            {
                var obj = Instantiate(model.GetSelector(o));
                view.SetParentSelector(obj.transform);
                storeSelector.Add(obj.GetComponent<Selector>());
            }

            // Assign callback for selector
            foreach (var arrow in storeSelector)
            {
                arrow.Init(OnClickedSelector);
            }

            // Play button
            playButton.onClick.AddListener(OnClickPlay);

            // Init view
            view.InitBoard(boardSize);

            // Init Candy
            candy = Instantiate(model.CandyModel).GetComponent<Candy>();
            candy.Init(model.CandySprites[Random.Range(0, model.CandySprites.Count)]);
            view.InitCandyPosition(candy.GetComponent<RectTransform>(), targetPosition);

            // Init player
            player = Instantiate(model.PlayerModel);
            playerPosition = startPosition;
            view.InitPlayerPosition(player.GetComponent<RectTransform>(), startPosition);
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
            }
            else // Valid pos
            {
                if (!storeSelected.Contains(selectedObject))
                {
                    storeSelected.Insert(CalculatedCurrentPosition(Input.mousePosition), selectedObject);
                }

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
            // check to make space
            HandleDisplayCalculate(mousePos);
        }

        private void ResetGame()
        {
            // Clear all things
            foreach (var selector in storeSelected)
            {
                SimplePool.Despawn(selector.gameObject);
            }

            playerPosition = startPosition;
            storeSelected.Clear();

            // Reset player position and candy
            view.InitPlayerPosition(player.GetComponent<RectTransform>(), startPosition);
            view.InitCandyPosition(candy.GetComponent<RectTransform>(), targetPosition);
        }

        #region Calulate func

        private bool CheckWin()
        {
            foreach (var item in storeSelected)
            {
                switch (item.SelectType)
                {
                    case SelectType.Up:
                        playerPosition += Vector2.up;
                        break;
                    case SelectType.Down:
                        playerPosition += Vector2.down;
                        break;
                    case SelectType.Left:
                        playerPosition += Vector2.left;
                        break;
                    case SelectType.Right:
                        playerPosition += Vector2.right;
                        break;
                    case SelectType.Collect:
                        if (playerPosition == targetPosition)
                        {
                            return true;
                        }

                        break;
                }
            }

            return false;
        }

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
                view.MakeEmptySpace(storeSelected.Select(o => o.RectTransform).ToList(),
                    CalculatedCurrentPosition(mousePos));
            }
            else
            {
                view.ReSortItemsSelected(storeSelected.Select(o => o.RectTransform).ToList());
            }
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

        #endregion

        #region CALL BACK

        // Event clicked selector
        private void OnClickedSelector(Selector selectedObj)
        {
            // Generate new selected
            var obj = SimplePool.Spawn(model.GetSelected(selectedObj.SelectType));
            view.SetParentSelected(obj.transform);
            // Generate init selected
            var arrow = obj.GetComponent<Selector>();
            arrow.Init(OnClickedSelected);
            // assign to control
            selectedObject = arrow;
            StoreTempPosition();
        }

        private void OnClickedSelected(Selector selectedObj)
        {
            storeSelected.Remove(selectedObj);
            selectedObject = selectedObj;
            view.ReSortItemsSelected(storeSelected.Select(o => o.RectTransform).ToList());
            StoreTempPosition();
        }

        // Start Moving
        private async void OnClickPlay()
        {
            playButton.interactable = false;
            var isWin = CheckWin();
            view.MovePlayer(
                storeSelected.Select(o => o.SelectType).ToList()
                , model.PlayerMoveTime);

            await Task.Delay((int)(model.PlayerMoveTime * storeSelected.Count * 1000));
            if (isWin)
            {
                candy.gameObject.SetActive(false);
            }
            else
            {
                Debug.Log("FAIL");
                ResetGame();
                playButton.interactable = true;
            }
        }

        #endregion
    }
}
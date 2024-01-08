using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameScene.Component;
using JetBrains.Annotations;
using MainScene.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utilities;

namespace GameScene.GameSequence
{
    public class SequenceController : GameController
    {
        [Header("Reference model")]
        [SerializeField] private SequenceView view;
        [SerializeField] private SequenceModel model;

        [Header("Reference object game")]
        [SerializeField] private RectTransform deleteZone;
        [SerializeField] private RectTransform selectedZone;
        [SerializeField] private Button playButton;

        [Header("Testing only")]
        [SerializeField] private List<SelectType> generateList;

        // FOR CONTROL SELECTOR
        private readonly List<Selector> storeSelector = new();
        private readonly List<Selector> storeSelected = new();
        private readonly List<Vector2> storedPosition = new();
        private bool isDelete;
        private readonly float offSet = 0.2f;
        [CanBeNull] private Selector selectedObject;

        // PLAY GROUND
        private Candy candy;

        // GAME DATA
        private LevelItemData levelData;
        private Vector2 currentPlayerPosition;
        private int stageIndex;
        private int levelIndex;
        private int coinWin;
        private int gemWin;
        private bool isPrevious;

        #region INITIALIZE

        private void Start()
        {
            // LoadData();
            CreateSelector();
            CreateBoard();
            CreateTarget();
            CreatePlayer();
            InitView();
        }

        private void LoadData()
        {
            var param = PopupHelpers.PassParamPopup();
            levelData = param.GetObject<LevelItemData>(ParamType.LevelData);
            stageIndex = param.GetObject<int>(ParamType.StageIndex);
            levelIndex = param.GetObject<int>(ParamType.LevelIndex);
            if (isPrevious)
            {
                coinWin = 0;
                gemWin = 0;
            }
            else
            {
                var reward1 = levelData.LevelReward.FirstOrDefault(o => o.RewardType == Enums.RewardType.Coin);
                var reward2 = levelData.LevelReward.FirstOrDefault(o => o.RewardType == Enums.RewardType.Coin);
                coinWin = reward1?.Value ?? 0;
                gemWin = reward2?.Value ?? 0;
            }

            boardSize = levelData.BoardSize;
            targetPosition = levelData.TargetPosition;
        }

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
            candy = Instantiate(model.TargetPrefab).GetComponent<Candy>();
            candy.Init(model.CandySprites[Random.Range(0, model.CandySprites.Count)]);
            view.PlaceObjectToBoard(candy.GetComponent<Transform>(), targetPosition);
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
                    ResetGame();
                    yield break;
                }

                targetMove = view.GetPositionFromBoard(targetMove);
                yield return MovePlayer(targetMove, model.PlayerMoveTime);
            }
            else
            {
                playerControl.PlayAnimationEat();
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

            // Reset player position and candy
            currentPlayerPosition = playerPosition;
            playerControl.RotatePlayer(
                targetPosition.x >= playerPosition.x
                , 0.1f);
            playerControl.PlayAnimationIdle();
            view.PlaceObjectToBoard(playerControl.transform, playerPosition);
        }

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

        private bool IsOutsideBoard(Vector2 checkPosition)
        {
            return checkPosition.x > boardSize.x || checkPosition.y > boardSize.y ||
                   checkPosition.x <= 0 || checkPosition.y <= 0;
        }

        #endregion

        #region CALL BACK

        // Event clicked selector
        private void OnClickedSelector(Selector selectedObj)
        {
            // Generate new selected
            var obj = SimplePool.Spawn(model.SelectedPrefab);

            // Generate init selected
            var arrow = obj.GetComponent<Arrow>();
            arrow.Init(OnClickedSelected);
            arrow.ChangeRender(model.GetSelected(selectedObj.SelectType));
            arrow.SelectType = selectedObj.SelectType;

            // assign to control
            selectedObject = arrow;
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

        /// <summary>
        /// Win popup
        /// </summary>
        private void OnClickClaim()
        {
            // Save data
            playerService.UserCoin += coinWin;
            playerService.UserDiamond += gemWin;
            playerService.SaveData();
            // Load level

            var param = PopupHelpers.PassParamPopup();
            param.SaveObject(ParamType.StageIndex, stageIndex);
            param.SaveObject("OpenPopup", true);
            SceneManager.LoadScene(Constants.MainMenu);
        }

        /// <summary>
        /// Win popups
        /// </summary>
        private void OnClickClaimAds()
        {
            // Load level
            var param = PopupHelpers.PassParamPopup();
            param.SaveObject(ParamType.StageIndex, stageIndex);
            SceneManager.LoadScene(Constants.MainMenu);
        }

        private void OnClickExit()
        {
            SceneManager.LoadScene(Constants.MainMenu);
        }

        #endregion

        private void ShowWinPopup(int numOfStar, int coinReward, int gemReward)
        {
            var param = PopupHelpers.PassParamPopup();
            param.SaveObject("Coin", coinReward);
            param.SaveObject("Gem", gemReward);
            param.SaveObject("NumberOfStars", numOfStar);
            param.SaveObject("Title", "Stage clear!");
            param.AddAction(ActionType.YesOption, OnClickClaim);
            param.AddAction(ActionType.AdsOption, OnClickClaimAds);
            param.AddAction(ActionType.QuitOption, OnClickExit);
            PopupHelpers.Show(Constants.WinPopup);
        }
    }
}
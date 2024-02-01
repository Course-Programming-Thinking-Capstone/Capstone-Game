using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameScene.Component;
using GameScene.Component.SelectControl;
using JetBrains.Annotations;
using Spine.Unity;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;

namespace GameScene.GameSequence
{
    public class SequenceController : ClickDragController
    {
        [Header("Reference model")]
        [SerializeField] private SequenceView view;
        [SerializeField] private SequenceModel model;



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

 

        #region Perform action

        private void HandleMouseUp()
        {
            if (storeSelected.Count == 15)
            {
                isDelete = true;
            }

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
            if (storeSelected.Count == 15)
            {
                isDelete = true;
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
                var item = storeSelected[i];
                view.SetParentSelectedToMove(item.transform);
                item.ActiveEffect();
                yield return HandleAction(item);
                item.ActiveEffect(false);
                view.SetParentSelected(item.transform);
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

        private IEnumerator HandleAction(Selector direction)
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
                    playerControl.PlayAnimationIdle();
                    yield return new WaitForSeconds(1f);
                    ResetGame();
                    yield break;
                }

                targetMove = view.GetPositionFromBoard(targetMove);
                yield return MovePlayer(targetMove, model.PlayerMoveTime);
            }

            if (isEat)
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

            if (isBreak)
            {
                playerControl.PlayAnimationIdle();
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

        #region Calulate func

        private int CalculatedCurrentPosition(Vector2 mousePos)
        {
            for (int i = 0; i < storedPosition.Count; i++)
            {
                if (i == 0 && storedPosition[i].y - OffSet < mousePos.y) // first item
                {
                    return 0;
                }

                if (i == storedPosition.Count - 1) // last item
                {
                    return storedPosition.Count;
                }

                if (storedPosition[i].y + OffSet > mousePos.y
                    && storedPosition[i + 1].y - OffSet < mousePos.y)
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
            // playerService.UserCoin += coinWin;
            // playerService.UserDiamond += gemWin;
            playerService.SaveData();
            // Load level

            var param = PopupHelpers.PassParamPopup();
            // param.SaveObject(ParamType.StageIndex, stageIndex);
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
            // param.SaveObject(ParamType.StageIndex, stageIndex);
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
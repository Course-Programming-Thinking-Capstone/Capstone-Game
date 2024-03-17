using Services;

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

        #endregion

        #region CALL BACK

        // Start Moving
        private void OnClickPlay()
        {
            StartCoroutine(StartPlayerMove());
        }

        #endregion
    }
}
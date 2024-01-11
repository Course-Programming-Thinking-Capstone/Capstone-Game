using UnityEngine;

namespace GameScene.GameBasic
{
    public class BasicView : GameView
    {
        
        [SerializeField] private Transform container1;
        [SerializeField] private Transform container2;
        [SerializeField] private Transform movingContainer;
        public int CountLeft { get; set; }
        public int CountRight { get; set; }

        public void AddRoadToContainer(Transform newObjectRoad)
        {
            if (CountLeft <= CountRight)
            {
                newObjectRoad.SetParent(container1);
                CountLeft++;
            }
            else
            {
                newObjectRoad.SetParent(container2);
                CountRight++;
            }
        }

        public void GetRoadToMove(Transform objectClicked)
        {
            if (objectClicked.parent == container1)
            {
                CountLeft--;
            }
            else
            {
                CountRight--;
            }

            objectClicked.SetParent(movingContainer);
        }

        public void InitGroundBoardFakePosition(Vector2 board, float offSet)
        {
            boardSize = board;
            var sizeY = (int)board.y;
            var sizeX = (int)board.x;
            for (int i = 0; i < sizeY; i++) // vertical
            {
                for (int j = 0; j < sizeX; j++) // horizontal
                {
                    var positionNew = startGroundPosition.position;
                    positionNew.x += offSet * j;
                    positionNew.y += offSet * i;
                    positions.Add(positionNew);
                }
            }
        }

        public Vector2 PlaceGround(Transform groundItem, Vector2 position)
        {
            groundItem.position = GetPositionFromBoard(position);
            groundItem.SetParent(blockContainer);
            return groundItem.position;
        }

        public void PlacePlayer(Transform playerTransform, Vector2 position)
        {
            playerTransform.position = GetPositionFromBoard(position);
        }

        private Vector2 GetPositionFromBoard(Vector2 position)
        {
            int index = (int)((position.y - 1) * boardSize.x + (position.x - 1));
            return positions[index];
        }
    }
}
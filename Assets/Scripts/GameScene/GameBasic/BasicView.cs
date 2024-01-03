using System.Collections.Generic;
using UnityEngine;

namespace GameScene.GameBasic
{
    public class BasicView : GameView
    {
        // 2D
        [SerializeField] private Transform startGroundPosition;
        [SerializeField] private Transform blockContainer;
        
        // Canvas
        [SerializeField] private Transform container1;
        [SerializeField] private Transform container2;
        [SerializeField] private Transform movingContainer;
        private int countLeft;
        private int countRight;

        public void AddRoadToContainer(Transform newObjectRoad)
        {
            if (countLeft <= countRight)
            {
                newObjectRoad.SetParent(container1);
                countLeft++;
            }
            else
            {
                newObjectRoad.SetParent(container2);
                countRight++;
            }
        }

        public void GetRoadToMove(Transform objectClicked)
        {
            if (objectClicked.parent == container1)
            {
                countLeft--;
            }
            else
            {
                countRight--;
            }
            objectClicked.SetParent(movingContainer);
        }
        
        public void InitGroundBoard(List<Transform> groundItems, Vector2 board, float offSet)
        {
            var sizeY = (int)board.y;
            var sizeX = (int)board.x;
            for (int i = 0; i < sizeY; i++) // vertical
            {
                for (int j = 0; j < sizeX; j++) // horizontal
                {
                    var positionNew = startGroundPosition.position;
                    positionNew.x += offSet * j;
                    positionNew.y += offSet * i;
                    groundItems[i * sizeX + j].position = positionNew;
                    groundItems[i * sizeX + j].SetParent(blockContainer);
                }
            }
        }
    }
}
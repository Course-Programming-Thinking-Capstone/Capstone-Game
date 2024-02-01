using UnityEngine;

namespace GameScene.GameBasic
{
    public class BasicView : GameView
    {
        [SerializeField] private Transform container1;
        [SerializeField] private Transform container2;
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
    }
}
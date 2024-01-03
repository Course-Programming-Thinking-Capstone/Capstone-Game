using UnityEngine;

namespace GameScene.GameBasic
{
    public class BasicView : GameView
    {
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
    }
}
using UnityEngine;

namespace GameScene.GameBasic
{
    public class BasicGameView : GameView
    {
        [SerializeField] private Transform disPlayContainer;
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
            else if (objectClicked.parent == container2)
            {
                CountRight--;
            }

            objectClicked.SetParent(disPlayContainer);
        }
    }
}
using UnityEngine;

namespace GameScene.GameBasic
{
    public class BasicGameView : MonoBehaviour
    {
        [SerializeField] protected GameObject savePanel;
        [SerializeField] private Transform disPlayContainer;
        [SerializeField] private Transform container1;
        [SerializeField] private Transform container2;
        public int CountLeft { get; set; }
        public int CountRight { get; set; }

        public void ActiveSavePanel(bool isActive = true)
        {
            savePanel.SetActive(isActive);
        }

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

            objectClicked.SetParent(disPlayContainer);
        }
    }
}
using UnityEngine;

namespace GameScene
{
    public class GameView : MonoBehaviour
    {
        [SerializeField] private GameObject savePanel;
        public void ActiveSavePanel(bool isActive = true)
        {
            savePanel.SetActive(isActive);
        }
    }
}
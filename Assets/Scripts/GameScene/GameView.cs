using TMPro;
using UnityEngine;

namespace GameScene
{
    public class GameView : MonoBehaviour
    {
        [Header("2D references")]
        [SerializeField] protected GameObject savePanel;
        [SerializeField] protected TextMeshProUGUI levelDetail;

        public void SetDetail(string detail)
        {
            levelDetail.text = detail;
        }
        public void ActiveSavePanel(bool isActive = true)
        {
            savePanel.SetActive(isActive);
        }
    }
}
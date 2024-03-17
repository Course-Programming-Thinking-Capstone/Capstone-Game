using System.Collections.Generic;
using UnityEngine;

namespace GameScene
{
    public class GameView : MonoBehaviour
    {
        [Header("2D references")]
        [SerializeField] protected GameObject savePanel;

        public void ActiveSavePanel(bool isActive = true)
        {
            savePanel.SetActive(isActive);
        }
    }
}
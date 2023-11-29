using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MainScene.MainPopup
{
    public class LevelSelect : MainScenePopup
    {
        [SerializeField] private Button homeButton;
        [SerializeField] private Button backButton;
        [SerializeField] private TextMeshProUGUI coinTxt;
        [SerializeField] private TextMeshProUGUI diamondTxt;
        [SerializeField] private GameObject modelGenerated;

        
        public void Initialized()
        {
            homeButton.onClick.AddListener(Close);
            backButton.onClick.AddListener(Close);
      
        }
    }
}

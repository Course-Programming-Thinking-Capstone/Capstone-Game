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
        
        public void Initialized()
        {
            homeButton.onClick.AddListener(Close);
            backButton.onClick.AddListener(Close);
      
        }
        public void SetWallet(int coin, int diamond)
        {
            coinTxt.text = coin.ToString();
            diamondTxt.text = diamond.ToString();
        }
    }
}

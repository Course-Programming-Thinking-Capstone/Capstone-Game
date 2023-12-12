using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MainScene.MainPopup
{
    public class LevelSelect : MainScenePopup
    {
        [SerializeField] private Button homeButton;
        [SerializeField] private Button backButton;
        [SerializeField] private TextMeshProUGUI coinTxt;
        [SerializeField] private TextMeshProUGUI diamondTxt;
        [SerializeField] private Transform contentContainer;

        public void Initialized(UnityAction closeAll)
        {
            homeButton.onClick.AddListener(closeAll);
            backButton.onClick.AddListener(Close);
      
        }
        public void SetWallet(int coin, int diamond)
        {
            coinTxt.text = coin.ToString();
            diamondTxt.text = diamond.ToString();
        }
        public void AddElement(Transform element)
        {
            element.SetParent(contentContainer);
        }
    }
}

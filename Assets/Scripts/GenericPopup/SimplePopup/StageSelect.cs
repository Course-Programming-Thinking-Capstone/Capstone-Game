using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GenericPopup.SimplePopup
{
    public class StageSelect : PopupAdditive
    {
        [SerializeField] private Button backButton;
        [SerializeField] private TextMeshProUGUI coinTxt;
        [SerializeField] private TextMeshProUGUI diamondTxt;
        [SerializeField] private Transform contentContainer;

        private void Start()
        {
            backButton.onClick.AddListener(ClosePopup);
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
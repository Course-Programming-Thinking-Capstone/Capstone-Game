using Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GenericPopup.SimplePopup
{
    public class GameModeSelect : PopupAdditive
    {
        [SerializeField] private Button backButton;
        [SerializeField] private TextMeshProUGUI coinTxt;
        [SerializeField] private TextMeshProUGUI energyTxt;
        [SerializeField] private Transform contentContainer;

        private APIService apiService;

        private void Start()
        {
            apiService = GameServices.Instance.GetService<APIService>();
            apiService = GameServices.Instance.GetService<APIService>();
            backButton.onClick.AddListener(ClosePopup);
            coinTxt.text = apiService.Coin.ToString();
            energyTxt.text = "60 / 60";
            
        }

        public void AddElement(Transform element)
        {
            element.SetParent(contentContainer);
        }
    }
}
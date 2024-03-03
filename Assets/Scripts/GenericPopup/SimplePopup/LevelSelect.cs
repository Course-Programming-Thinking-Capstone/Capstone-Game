using Services;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GenericPopup.SimplePopup
{
    public class LevelSelect : PopupAdditive
    {
        [SerializeField] private Button backButton;
        [SerializeField] private TextMeshProUGUI coinTxt;
        [SerializeField] private TextMeshProUGUI energyTxt;
        [SerializeField] private TextMeshProUGUI modeName;
        [SerializeField] private Transform contentContainer;

        private APIService apiService;

        private void Start()
        {
            apiService = GameServices.Instance.GetService<APIService>();
            backButton.onClick.AddListener(ClosePopup);
            coinTxt.text = apiService.Coin.ToString();
            energyTxt.text = "60 / 60";
            modeName.text = "Mode name here";
        }

        public void AddElement(Transform element)
        {
            element.SetParent(contentContainer);
        }
    }
}
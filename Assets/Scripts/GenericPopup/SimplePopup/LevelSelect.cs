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

        private ServerSideService serverSideService;

        private void Start()
        {
            serverSideService = GameServices.Instance.GetService<ServerSideService>();
            backButton.onClick.AddListener(ClosePopup);
            coinTxt.text = serverSideService.Coin.ToString();
            energyTxt.text = "60 / 60";
            modeName.text = "Mode name here";
        }

        public void AddElement(Transform element)
        {
            element.SetParent(contentContainer);
        }
    }
}
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
        [SerializeField] private Image modeBg;

        private ServerSideService serverSideService;

        private void Start()
        {
            serverSideService = GameServices.Instance.GetService<ServerSideService>();
            backButton.onClick.AddListener(ClosePopup);
            coinTxt.text = serverSideService.Coin.ToString();
            energyTxt.text = "60 / 60";
            
        }

        public void AddElement(Transform element)
        {
            element.SetParent(contentContainer);
        }
    }
}
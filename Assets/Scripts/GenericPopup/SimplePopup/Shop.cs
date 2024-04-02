using AYellowpaper.SerializedCollections;
using Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GenericPopup.SimplePopup
{
    public class Shop : PopupAdditive
    {
        [SerializeField]
        [SerializedDictionary("RateType", "Sprite color")]
        private SerializedDictionary<Enums.RateType, Sprite> rateRender;
        [SerializeField] private Image rateImage;
        [SerializeField] private TextMeshProUGUI characterNameTxt;
        [SerializeField] private TextMeshProUGUI characterDetailTxt;
        [SerializeField] private GameObject shopPrefab;
        [SerializeField] private TextMeshProUGUI energyTxt;
        [SerializeField] private TextMeshProUGUI coinTxt;
        [SerializeField] private TextMeshProUGUI gemTxt;
        private ClientService clientService;

        private void Awake()
        {
            clientService = GameServices.Instance.GetService<ClientService>();
        }

        private void Start()
        {
            coinTxt.text = clientService.Coin.ToString();
            gemTxt.text = clientService.Gem.ToString();
            energyTxt.text = "60 / 60";

            loading.SetActive(true);
        }

        public void OnClickClose()
        {
            ClosePopup();
        }
    }
}
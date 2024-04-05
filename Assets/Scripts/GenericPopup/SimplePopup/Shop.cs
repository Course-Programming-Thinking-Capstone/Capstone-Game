using AYellowpaper.SerializedCollections;
using Data;
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
        [SerializeField] private TextMeshProUGUI rankTxt;
        [SerializeField] private TextMeshProUGUI characterNameTxt;
        [SerializeField] private TextMeshProUGUI characterDetailTxt;
        [SerializeField] private TextMeshProUGUI coinTxt;
        [SerializeField] private TextMeshProUGUI gemTxt;
        [SerializeField] private CharacterData shopData;
        private ClientService clientService;

        private void Awake()
        {
            clientService = GameServices.Instance.GetService<ClientService>();
        }

        private void Start()
        {
            coinTxt.text = clientService.Coin.ToString();
            gemTxt.text = clientService.Gem.ToString();

            loading.SetActive(true);
        }

        public void OnClickClose()
        {
            ClosePopup();
        }
    }
}
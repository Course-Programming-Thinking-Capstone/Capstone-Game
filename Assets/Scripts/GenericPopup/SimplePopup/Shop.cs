using Services;
using TMPro;
using UnityEngine;

namespace GenericPopup.SimplePopup
{
    public class Shop : PopupAdditive
    {
        [SerializeField] private GameObject itemPrefab;
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
    }
}
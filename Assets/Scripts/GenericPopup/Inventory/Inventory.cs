using Services;
using TMPro;
using UnityEngine;

namespace GenericPopup.Inventory
{
    public class Inventory : PopupAdditive
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
        }

        public void OnClickClose()
        {
            ClosePopup();
        }

        public void OnClickSold()
        {
        }

        public void OnClickViewItem()
        {
        }
    }
}
using Services;
using TMPro;
using UnityEngine;

namespace GenericPopup.Inventory
{
    public class Inventory : PopupAdditive
    {
        [SerializeField] private Transform itemContainer;
        [SerializeField] private GameObject itemPrefab;
        [SerializeField] private TextMeshProUGUI energyTxt;
        [SerializeField] private TextMeshProUGUI coinTxt;
        [SerializeField] private TextMeshProUGUI gemTxt;
        private ClientService clientService;
        [SerializeField] private string resourcesPath;

        private void Awake()
        {
            clientService = GameServices.Instance.GetService<ClientService>();
        }

        private void Start()
        {
            coinTxt.text = clientService.Coin.ToString();
            gemTxt.text = clientService.Gem.ToString();
            energyTxt.text = "60 / 60";

            for (int i = 0; i < 10; i++)
            {
                var fakeIt = CreateItem();
                fakeIt.InitializedItem(
                    (Enums.RateType)Random.Range(1, 7),
                    Resources.Load<Sprite>(resourcesPath + "T_fruit_" + Random.Range(1, 15)),
                    Random.Range(1, 7),
                    () => { Debug.Log("CLICKED"); }
                );
            }

            loading.SetActive(false);
        }

        public InventItem CreateItem()
        {
            var obj = Instantiate(itemPrefab, itemContainer);
            obj.transform.localScale = Vector3.one;
            return obj.GetComponent<InventItem>();
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
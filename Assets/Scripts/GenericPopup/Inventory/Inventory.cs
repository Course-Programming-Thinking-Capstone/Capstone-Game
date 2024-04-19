using AYellowpaper.SerializedCollections;
using Services;
using Services.Response;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GenericPopup.Inventory
{
    public class Inventory : PopupAdditive
    {
        [SerializeField]
        [SerializedDictionary("RateType", "Sprite color")]
        private SerializedDictionary<Enums.RateType, Sprite> rateRender;
        [Header("Header infor")]
        [SerializeField] private TextMeshProUGUI energyTxt;
        [SerializeField] private TextMeshProUGUI coinTxt;
        [SerializeField] private TextMeshProUGUI gemTxt;

        [Header("Items")]
        [SerializeField] private Transform itemContainer;
        [SerializeField] private GameObject itemPrefab;

        [Header("Item details")]
        [SerializeField] private Image imageDetailRate;
        [SerializeField] private Image imageDetail;
        [SerializeField] private TextMeshProUGUI countTxt;
        [SerializeField] private TextMeshProUGUI rateTxt;
        [SerializeField] private TextMeshProUGUI itemNameTxt;
        [SerializeField] private TextMeshProUGUI itemDetailTxt;
        [SerializeField] private TextMeshProUGUI sellValueTxt;
        private ClientService clientService;
        [Header("System")]
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
                    rateRender[(Enums.RateType)Random.Range(1, 7)],
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

        public void HideDetail()
        {
            
        }
        public void SetCurrentDetail(GameItemResponse model, int numberHave)
        {
            var render = Resources.Load<Sprite>(resourcesPath + model.SpritesUrl);
            imageDetailRate.sprite = rateRender[(Enums.RateType)model.ItemRateType];
            imageDetail.sprite = render;
            countTxt.text = numberHave.ToString();
            rateTxt.text = ((Enums.RateType)model.ItemRateType).ToString();
            itemNameTxt.text = model.ItemName;
            itemDetailTxt.text = model.Details;
            sellValueTxt.text = model.Price.ToString();
        }
    }
}
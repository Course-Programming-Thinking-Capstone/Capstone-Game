using System.Collections.Generic;
using System.Threading.Tasks;
using AYellowpaper.SerializedCollections;
using Services;
using Services.Response;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace GenericPopup.Inventory
{
    public class Inventory : PopupAdditive
    {
        [SerializeField]
        [SerializedDictionary("RateType", "Sprite color")]
        private SerializedDictionary<Enums.RateType, Sprite> rateRender;

        [Header("Solving")]
        [SerializeField] private TextMeshProUGUI soldCount;
        [SerializeField] private TextMeshProUGUI sellValueTxt;

        [Header("ConfirmSold")]
        [SerializeField] private GameObject confirmSold;
        [SerializeField] private TextMeshProUGUI detailConfirmSold;

        [Header("Header infor")]
        [SerializeField] private TextMeshProUGUI energyTxt;
        [SerializeField] private TextMeshProUGUI coinTxt;
        [SerializeField] private TextMeshProUGUI gemTxt;

        [Header("Items")]
        [SerializeField] private Transform itemContainer;
        [SerializeField] private GameObject itemPrefab;

        [Header("Item details")]
        [SerializeField] private GameObject maskItemObj;
        [SerializeField] private GameObject detailObj;
        [SerializeField] private Image imageDetailRate;
        [SerializeField] private Image imageDetail;
        [SerializeField] private TextMeshProUGUI countTxt;
        [SerializeField] private TextMeshProUGUI rateTxt;
        [SerializeField] private TextMeshProUGUI itemNameTxt;
        [SerializeField] private TextMeshProUGUI itemDetailTxt;
        private ClientService clientService;
        private AudioService audioService;
        [Header("System")]
        [SerializeField] private string resourcesPath;
        private List<InventItem> cache = new();
        private int selectedId;
        private int soldNumber;
        private int numberHave;
        private int price;

        private void Awake()
        {
            clientService = GameServices.Instance.GetService<ClientService>();
            audioService = GameServices.Instance.GetService<AudioService>();
        }

        private void Start()
        {
            audioService.PlaySound(GUISound.Popup);
            coinTxt.text = clientService.Coin.ToString();
            gemTxt.text = clientService.Gem.ToString();
            energyTxt.text = "60 / 60";
            maskItemObj.SetActive(true);
            detailObj.SetActive(false);
            LoadItem();
        }

        private async void LoadItem()
        {
            loading.SetActive(true);
            foreach (var item in cache)
            {
                Destroy(item.gameObject);
            }

            cache.Clear();
            var inventItem = await clientService.GetUserOwnedItems();
            if (inventItem != null && inventItem.Count > 0)
            {
                foreach (var item in inventItem)
                {
                    var temp = item;
                    var itemObj = CreateItem();
                    cache.Add(itemObj);
                    itemObj.InitializedItem(
                        rateRender[(Enums.RateType)item.GameItem.ItemRateType],
                        Resources.Load<Sprite>(resourcesPath + item.GameItem.SpritesUrl),
                        item.Quantity,
                        () =>
                        {
                            foreach (var inventItemCache in cache)
                            {
                                inventItemCache.SetFocus(false);
                            }

                            SetCurrentDetail(temp.GameItem, item.Quantity);
                        }
                    );
                }
            }

            loading.SetActive(false);
        }

        private InventItem CreateItem()
        {
            var obj = Instantiate(itemPrefab, itemContainer);
            obj.transform.localScale = Vector3.one;
            return obj.GetComponent<InventItem>();
        }

        public void OnClickClose()
        {
            audioService.PlaySound(GUISound.Click);
            ClosePopup();
        }

        public void OnClickRight()
        {
            audioService.PlaySound(GUISound.Click);
            if (soldNumber >= numberHave)
            {
                return;
            }

            soldNumber++;
            UpdateSoldValue();
        }

        public void OnClickLeft()
        {
            audioService.PlaySound(GUISound.Click);
            if (soldNumber <= 1)
            {
                return;
            }

            soldNumber--;
            UpdateSoldValue();
        }

        private void UpdateSoldValue()
        {
            soldCount.text = soldNumber.ToString();
            sellValueTxt.text = (soldNumber * price).ToString();
        }

        public void OnClickSold()
        {
            audioService.PlaySound(GUISound.Click);
            audioService.PlaySound(GUISound.Popup);
            detailConfirmSold.text = "Are you sure to sold this item for: " + price * soldNumber + " Gems?";
            confirmSold.SetActive(true);
        }

        public void ConfirmSold()
        {
            audioService.PlaySound(GUISound.Click);
            loading.SetActive(true);
            clientService.SellItem(selectedId, soldNumber, o =>
            {
                loading.SetActive(false);
                LoadItem();
                coinTxt.text = clientService.Coin.ToString();
                gemTxt.text = clientService.Gem.ToString();
                maskItemObj.SetActive(true);
                detailObj.SetActive(false);
                audioService.PlaySound(GUISound.Success);
                PopupHelpers.ShowError("Sold success", "Notification");
            }, e =>
            {
                audioService.PlaySound(GUISound.Fail);
                loading.SetActive(false);
                PopupHelpers.ShowError(e);
            });
        }

        public void CloseConfirm()
        {
            audioService.PlaySound(GUISound.Click);
            confirmSold.SetActive(false);
        }

        private void SetCurrentDetail(GameItemResponse model, int numberHaveParam)
        {
            audioService.PlaySound(GUISound.Click);
            maskItemObj.SetActive(false);
            detailObj.SetActive(true);
            var render = Resources.Load<Sprite>(resourcesPath + model.SpritesUrl);
            soldNumber = 1;
            numberHave = numberHaveParam;
            UpdateSoldValue();
            selectedId = model.Id;
            imageDetailRate.sprite = rateRender[(Enums.RateType)model.ItemRateType];
            imageDetail.sprite = render;
            countTxt.text = numberHave.ToString();
            rateTxt.text = ((Enums.RateType)model.ItemRateType).ToString();
            itemNameTxt.text = model.ItemName;
            itemDetailTxt.text = model.Details;
            sellValueTxt.text = model.Price.ToString();
            price = model.Price;
        }
    }
}
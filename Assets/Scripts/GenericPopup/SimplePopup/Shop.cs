using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace GenericPopup.SimplePopup
{
    public class Shop : PopupAdditive
    {
        [SerializeField]
        [SerializedDictionary("RateType", "Sprite color")]
        private SerializedDictionary<Enums.RateType, Sprite> rateRender;
        [SerializeField] private Transform characterContainer;
        [SerializeField] private Image rateImage;
        [SerializeField] private TextMeshProUGUI rankTxt;
        [SerializeField] private TextMeshProUGUI characterNameTxt;
        [SerializeField] private TextMeshProUGUI characterDetailTxt;
        [SerializeField] private TextMeshProUGUI buyPriceTxt;
        [SerializeField] private GameObject buyButton;
        [SerializeField] private GameObject selectButton;
        [SerializeField] private TextMeshProUGUI coinTxt;
        [SerializeField] private TextMeshProUGUI gemTxt;

        [SerializeField] private GameObject errorModel;

        private GameObject currentChar;
        private ClientService clientService;
        private PlayerService playerService;
        private int currentIndex = 0;
        private int maxIndex = 0;

        private void Awake()
        {
            clientService = GameServices.Instance.GetService<ClientService>();
            playerService = GameServices.Instance.GetService<PlayerService>();
        }

        private async void Start()
        {
            coinTxt.text = clientService.Coin.ToString();
            gemTxt.text = clientService.Gem.ToString();

            if (clientService.CacheShopData == null)
            {
                loading.SetActive(true);
                await clientService.GetShopData();
                loading.SetActive(false);
            }

            if (clientService.CacheShopData == null)
            {
                PopupHelpers.ShowError("Shop data load error, please try again later");
                return;
            }

            clientService.UserOwnedShopItem ??= new List<int>();

            maxIndex = clientService.CacheShopData.Count - 1;
            LoadCharacterData();
        }

        private void LoadCharacterData()
        {
            var character = clientService.CacheShopData[currentIndex];
            rateImage.sprite = rateRender[(Enums.RateType)character.ItemRateType];
            rankTxt.text = ((Enums.RateType)character.ItemRateType).ToString();
            characterNameTxt.text = character.ItemName;
            characterDetailTxt.text = character.Details;
            buyPriceTxt.text = character.Price.ToString();
            if (currentChar != null)
            {
                SimplePool.Despawn(currentChar);
            }

            var characterModel = Resources.Load<GameObject>("ShopCharacters/" + character.SpritesUrl);

            if (characterModel == null)
            {
                characterModel = errorModel;
            }

            currentChar = SimplePool.Spawn(characterModel, Vector3.zero, Quaternion.identity);
            currentChar.transform.SetParent(characterContainer);
            currentChar.transform.localScale = Vector3.one;

            if (character.ItemRateType == 0
                || (clientService.UserOwnedShopItem != null
                    && clientService.UserOwnedShopItem.Contains(character.Id)))
            {
                buyButton.SetActive(false);
                selectButton.SetActive(true);
            }
            else
            {
                buyButton.SetActive(true);
                selectButton.SetActive(false);
            }
        }

        public void OnClickSelect()
        {
            PopupHelpers.ShowError("Character equipped ", "Notification");
            selectButton.SetActive(false);
        }

        public async void OnClickBuy()
        {
            if (!clientService.IsLogin)
            {
                PopupHelpers.ShowError(
                    "Thanks for playing, You need to register and upgrade to a student account to continue learning without limitations.",
                    "Notification");
                return;
            }

            ActiveSafePanel(true);
            var itemId = clientService.CacheShopData[currentIndex].Id;
            var result = await clientService.BuyItem(itemId);
            if (result != null)
            {
                coinTxt.text = result.CurrentCoin.ToString();
                gemTxt.text = result.CurrentGem.ToString();
                PopupHelpers.ShowError("Buy successfully", "Congratulation");
                buyButton.SetActive(false);
                selectButton.SetActive(true);
            }

            ActiveSafePanel(false);
        }

        public void OnClickPrevious()
        {
            currentIndex--;
            if (currentIndex < 0)
            {
                currentIndex = maxIndex;
            }

            LoadCharacterData();
        }

        public void OnClickNext()
        {
            currentIndex++;
            if (currentIndex > maxIndex)
            {
                currentIndex = 0;
            }

            LoadCharacterData();
        }

        public void OnClickClose()
        {
            ClosePopup();
        }
    }
}
using AYellowpaper.SerializedCollections;
using Services;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
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
        private AudioService audioService;
        private int currentIndex = 0;
        private int maxIndex = 0;
        private UnityAction onSelectedNew;

        private void Awake()
        {
            clientService = GameServices.Instance.GetService<ClientService>();
            playerService = GameServices.Instance.GetService<PlayerService>();
            audioService = GameServices.Instance.GetService<AudioService>();
            var param = PopupHelpers.PassParamPopup();
            onSelectedNew = param.GetAction(PopupKey.CallBack);
            clientService.OnFailed = err =>
            {
                ActiveSafePanel(false);
                PopupHelpers.ShowError(err, "ERROR");
            };
        }

        private async void Start()
        {
            audioService.PlaySound(GUISound.Popup);
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

            if (clientService.UserOwnedShopItem == null || clientService.UserOwnedShopItem.Count == 0)
            {
                loading.SetActive(true);
                await clientService.GetShopOwnedData();
                loading.SetActive(false);
            }

            maxIndex = clientService.CacheShopData.Count - 1;
            LoadCharacterData();
        }

        private void LoadCharacterData()
        {
            if (clientService.CacheShopData == null || clientService.CacheShopData.Count == 0)
            {
                PopupHelpers.ShowError("Dose not have any shop item");
                return;
            }

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

            // Set default character
            if (playerService.SelectedCharacter == -1 && character.ItemRateType == 0)
            {
                playerService.SaveSelectedCharacter(character.Id);
                selectButton.SetActive(false);
            }

            // Set default if not login
            if (!clientService.IsLogin && character.ItemRateType == 0)
            {
                selectButton.SetActive(false);
            }

            // trigger selected character to display select button or not
            if (playerService.SelectedCharacter == character.Id)
            {
                selectButton.SetActive(false);
            }
        }

        public void OnClickSelect()
        {
            audioService.PlaySound(GUISound.Success);
            playerService.SaveSelectedCharacter(clientService.CacheShopData[currentIndex].Id);
            onSelectedNew?.Invoke();
            PopupHelpers.ShowError("Character equipped ", "Notification");
            selectButton.SetActive(false);
        }

        public async void OnClickBuy()
        {
            audioService.PlaySound(GUISound.Click);
            if (!clientService.IsLogin)
            {
                audioService.PlaySound(GUISound.Fail);
                PopupHelpers.ShowError(
                    "You need to register and upgrade to a student account to continue perform this action.",
                    "Notification");
                return;
            }

            ActiveSafePanel(true);
            var itemId = clientService.CacheShopData[currentIndex].Id;
            var result = await clientService.BuyItem(itemId);
            ActiveSafePanel(false);
            if (result != null)
            {
                coinTxt.text = result.CurrentCoin.ToString();
                gemTxt.text = result.CurrentGem.ToString();
                audioService.PlaySound(GUISound.Success);
                PopupHelpers.ShowError("Buy successfully", "Congratulation");
                buyButton.SetActive(false);
                selectButton.SetActive(true);
            }
        }

        public void OnClickPrevious()
        {
            currentIndex--;
            if (currentIndex < 0)
            {
                currentIndex = maxIndex;
            }

            audioService.PlaySound(GUISound.Click);
            LoadCharacterData();
        }

        public void OnClickNext()
        {
            currentIndex++;
            if (currentIndex > maxIndex)
            {
                currentIndex = 0;
            }

            audioService.PlaySound(GUISound.Click);
            LoadCharacterData();
        }

        public void OnClickClose()
        {
            audioService.PlaySound(GUISound.Click);
            ClosePopup();
        }
    }
}
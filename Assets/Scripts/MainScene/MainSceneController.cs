using System.Linq;
using Services;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace MainScene
{
    public class MainSceneController : MonoBehaviour
    {
        [Header("MVC")]
        [SerializeField] private MainSceneView view;
        [Header("MainMenu Button")]
        [SerializeField] private Button playButton;
        [SerializeField] private Button characterButton;
        [SerializeField] private Button shopButton;
        [SerializeField] private Button inventoryButton;
        [SerializeField] private Button userButton;
        [SerializeField] private Button settingButton;
        [SerializeField] private Transform spineContainer;
        [SerializeField] private GameObject defaultCharacter;
        [Header("SYSTEM")]
        private PlayerService playerService;
        private ClientService clientService;
        private AudioService audioService;

        private void Awake()
        {
            var gameServices = GameServices.Instance;
            playerService = gameServices.GetService<PlayerService>();
            clientService = gameServices.GetService<ClientService>();
            audioService = gameServices.GetService<AudioService>();
        }

        private void Start()
        {
            // main 
            audioService.PlayMusic(MusicToPlay.Menu);
            InitMain();
            AssignButton();
            LoadCharacter();
        }

        private GameObject currentDisplay;

        private async void LoadCharacter()
        {
            defaultCharacter.SetActive(true);
            if (currentDisplay != null)
            {
                SimplePool.Despawn(currentDisplay);
            }

            // not login
            if (!clientService.IsLogin)
            {
                return;
            }

            // not select new
            if (playerService.SelectedCharacter == -1)
            {
                return;
            }

            // not have shop data
            if (clientService.CacheShopData == null)
            {
                await clientService.GetShopData();
            }

            // still not have shop data (load fail)
            if (clientService.CacheShopData == null || clientService.CacheShopData.Count == 0)
            {
                return;
            }

            var character =
                clientService.CacheShopData.FirstOrDefault(o => o.Id == playerService.SelectedCharacter);
            if (character != null)
            {
                var characterModel = Resources.Load<GameObject>("ShopCharacters/" + character.SpritesUrl);
                if (characterModel != null)
                {
                    currentDisplay = SimplePool.Spawn(characterModel, Vector3.zero, Quaternion.identity);
                    currentDisplay.transform.SetParent(spineContainer);
                    currentDisplay.transform.localScale = Vector3.one;
                    currentDisplay.transform.localPosition = Vector3.one;
                    defaultCharacter.SetActive(false);
                }
            }
        }

        #region Initialized

        private void AssignButton()
        {
            userButton.onClick.AddListener(OnClickUser);
            characterButton.onClick.AddListener(OnClickCharacter);
            settingButton.onClick.AddListener(OnClickSetting);
            inventoryButton.onClick.AddListener(OnClickInventory);
            playButton.onClick.AddListener(OnClickPlay);
            shopButton.onClick.AddListener(OnClickShop);
        }

        private void InitMain()
        {
            if (clientService.UserId != -1)
            {
                view.SetDisplayUserCoin(clientService.Coin);
                view.SetDisplayUserGem(clientService.Gem);
                view.SetDisplayUserName(clientService.UserDisplayName);
            }
            else
            {
                view.SetDisplayUserCoin(0);
                view.SetDisplayUserName("Guest");
            }

            view.SetDisplayUserEnergy(60, 60);
        }

        #endregion

        #region CallBack

        private void OnClickPlay()
        {
            audioService.PlaySound(SoundToPlay.Play);
            PopupHelpers.Show(Constants.GameModePopup);
        }

        private void OnClickSetting()
        {
            audioService.PlaySound(SoundToPlay.Popup);
            var param = PopupHelpers.PassParamPopup();
            param.AddAction(PopupKey.CallBack, () =>
            {
                LoadCharacter();
                view.SetDisplayUserCoin(0);
                view.SetDisplayUserName("Guest");
            });
            PopupHelpers.Show(Constants.SettingPopup);
        }

        private void OnClickInventory()
        {
            audioService.PlaySound(SoundToPlay.Popup);
            if (clientService.IsLogin)
            {
                var param = PopupHelpers.PassParamPopup();
                param.AddAction(PopupKey.CallBack, () =>
                {
                    view.SetDisplayUserCoin(clientService.Coin);
                    view.SetDisplayUserGem(clientService.Gem);
                });
            }

            PopupHelpers.Show(Constants.InventoryPopup);
        }

        private void OnClickCharacter()
        {
            audioService.PlaySound(SoundToPlay.Popup);
            var param = PopupHelpers.PassParamPopup();
            param.AddAction(PopupKey.CallBack, () =>
            {
                LoadCharacter();
                if (clientService.IsLogin)
                {
                    view.SetDisplayUserCoin(clientService.Coin);
                    view.SetDisplayUserGem(clientService.Gem);
                }
            });
            PopupHelpers.Show(Constants.ShopPopup);
        }

        /// <summary>
        /// Shop voucher and coin
        /// </summary>
        private void OnClickShop()
        {
            audioService.PlaySound(SoundToPlay.Popup);
            if (clientService.IsLogin)
            {
                var param = PopupHelpers.PassParamPopup();
                param.AddAction(PopupKey.CallBack, () =>
                {
                    view.SetDisplayUserCoin(clientService.Coin);
                    view.SetDisplayUserGem(clientService.Gem);
                });
            }

            PopupHelpers.Show(Constants.ShopVoucher);
        }

        private void OnClickUser()
        {
            audioService.PlaySound(SoundToPlay.Popup);
            if (clientService.UserId == -1)
            {
                var param = PopupHelpers.PassParamPopup();
                param.AddAction(PopupKey.CallBack, () =>
                {
                    view.SetDisplayUserCoin(clientService.Coin);
                    view.SetDisplayUserGem(clientService.Gem);
                    view.SetDisplayUserName(clientService.UserDisplayName);
                    LoadCharacter();
                });

                PopupHelpers.Show(Constants.LoginPopup);
            }
            else
            {
                PopupHelpers.Show(Constants.ProfilePopup);
            }
        }

        #endregion
    }
}
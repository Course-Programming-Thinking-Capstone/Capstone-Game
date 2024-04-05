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
        [SerializeField] private Button shopButton;
        [SerializeField] private Button inventoryButton;
        [SerializeField] private Button userButton;
        [SerializeField] private Button settingButton;
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
            InitMain();
            AssignButton();
        }

        #region Initialized

        private void AssignButton()
        {
            userButton.onClick.AddListener(OnClickUser);
            shopButton.onClick.AddListener(OnClickShop);
            settingButton.onClick.AddListener(OnClickSetting);
            inventoryButton.onClick.AddListener(OnClickInventory);
            playButton.onClick.AddListener(OnClickPlay);
        }

        private void InitMain()
        {
            if (clientService.UserId != -1)
            {
                view.SetDisplayUserCoin(clientService.Coin);
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
            PopupHelpers.Show(Constants.GameModePopup);
        }

        private void OnClickSetting()
        {
            var param = PopupHelpers.PassParamPopup();
            param.AddAction(PopupKey.CallBack, () =>
            {
                view.SetDisplayUserCoin(0);
                view.SetDisplayUserName("Guest");
            });
            PopupHelpers.Show(Constants.SettingPopup);
        }

        private void OnClickInventory()
        {
            PopupHelpers.Show(Constants.InventoryPopup);
        }

        private void OnClickShop()
        {
            PopupHelpers.Show(Constants.ShopPopup);
        }

        private void OnClickUser()
        {
            if (clientService.UserId == -1)
            {
                var param = PopupHelpers.PassParamPopup();
                param.AddAction(PopupKey.CallBack, () =>
                {
                    view.SetDisplayUserCoin(clientService.Coin);
                    view.SetDisplayUserName(clientService.UserDisplayName);
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
using Services;
using TMPro;
using UnityEngine;
using Utilities;

namespace GenericPopup.SimplePopup
{
    public class ShopVoucher : PopupAdditive
    {
        [SerializeField] private TextMeshProUGUI coinTxt;
        [SerializeField] private TextMeshProUGUI gemTxt;

        private ClientService clientService;
        private PlayerService playerService;

        private void Awake()
        {
            clientService = GameServices.Instance.GetService<ClientService>();
            playerService = GameServices.Instance.GetService<PlayerService>();
            var param = PopupHelpers.PassParamPopup();
            clientService.OnFailed = err =>
            {
                ActiveSafePanel(false);
                PopupHelpers.ShowError(err, "ERROR");
            };
        }

        private void Start()
        {
            coinTxt.text = clientService.Coin.ToString();
            gemTxt.text = clientService.Gem.ToString();

        }

        public async void OnClickBuy(int index)
        {
            if (!clientService.IsLogin)
            {
                PopupHelpers.ShowError(
                    "You need to register and upgrade to a student account to continue perform this action.",
                    "Notification");
                return;
            }

            ActiveSafePanel(true);
            var itemId = 0;
            var result = await clientService.BuyItem(itemId);
            ActiveSafePanel(false);
            if (result != null)
            {
                coinTxt.text = result.CurrentCoin.ToString();
                gemTxt.text = result.CurrentGem.ToString();
                PopupHelpers.ShowError("Buy successfully", "Congratulation");
            }
        }

        public void OnClickClose()
        {
            ClosePopup();
        }
    }
}
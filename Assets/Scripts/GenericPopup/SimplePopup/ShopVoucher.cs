using Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace GenericPopup.SimplePopup
{
    public class ShopVoucher : PopupAdditive
    {
        
        [SerializeField] private Image rateImage;
        [SerializeField] private GameObject buyButton;
        [SerializeField] private GameObject selectButton;
        [SerializeField] private TextMeshProUGUI coinTxt;
        [SerializeField] private TextMeshProUGUI gemTxt;

        private ClientService clientService;
        private PlayerService playerService;

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
                buyButton.SetActive(false);
                selectButton.SetActive(true);
            }
        }
        public void OnClickClose()
        {
            ClosePopup();
        }
    }
}
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Services;
using Services.Response;
using TMPro;
using UnityEngine;
using Utilities;

namespace GenericPopup.SimplePopup
{
    public class ShopVoucher : PopupAdditive
    {
        [SerializeField] private TextMeshProUGUI coinTxt;
        [SerializeField] private TextMeshProUGUI gemTxt;
        [Header("ConfirmBuy")]
        [SerializeField] private GameObject confirmBuy;
        [SerializeField] private TextMeshProUGUI detailConfirmBuy;

        [Header("View")]
        [SerializeField] private TextMeshProUGUI voucher1TxtPrice;
        [SerializeField] private TextMeshProUGUI voucher1CountLeft;

        [SerializeField] private TextMeshProUGUI voucher2TxtPrice;
        [SerializeField] private TextMeshProUGUI voucher2CountLeft;

        [SerializeField] private TextMeshProUGUI voucher3TxtPrice;
        [SerializeField] private TextMeshProUGUI voucher3CountLeft;

        [SerializeField]
        [SerializedDictionary("index", "price")]
        private SerializedDictionary<int, int> priceDictionary;
        [SerializeField]
        [SerializedDictionary("index", "value")]
        private SerializedDictionary<int, int> thingToGet;
        [SerializeField]
        private List<int> voucherIndex;
        private ClientService clientService;
        private PlayerService playerService;

        private void Awake()
        {
            animator.gameObject.SetActive(false);
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

            voucher1TxtPrice.text = priceDictionary[1]+ " Left";
            voucher2TxtPrice.text = priceDictionary[2]+ " Left";
            voucher3TxtPrice.text = priceDictionary[3]+ " Left";

            if (clientService.IsLogin)
            {
                voucher1CountLeft.text = playerService.LoadVoucherBoughtLeft(clientService.UserId, 1).ToString();
                voucher2CountLeft.text = playerService.LoadVoucherBoughtLeft(clientService.UserId, 2).ToString();
                voucher3CountLeft.text = playerService.LoadVoucherBoughtLeft(clientService.UserId, 3).ToString();
            }

            animator.gameObject.SetActive(true);
        }

        private int tempIndex;

        public void OnClickBuy(int index)
        {
            if (!clientService.IsLogin)
            {
                PopupHelpers.ShowError(
                    "You need to register and upgrade to a student account to continue perform this action.",
                    "Notification");
                return;
            }

            detailConfirmBuy.text = "Are you sure want to buy this item for " + priceDictionary[index] + " Gems?";
            tempIndex = index;
            confirmBuy.SetActive(true);
        }

        public void ConfirmBuy()
        {
            ActiveSafePanel(true);
            var itemId = tempIndex;

            if (voucherIndex.Contains(tempIndex))
            {
                // Buy voucher
                 clientService.BuyVoucher(itemId, priceDictionary[itemId], s =>
                {
                    
                    PopupHelpers.Show("Buy Success, Thank for your purchase");
                }, e =>
                {
                    
                });
            }
            else
            {
                // buy gold
                // result =  clientService.BuyVoucher(itemId);
            }

            ActiveSafePanel(false);

            // if (result != null)
            // {
            //     coinTxt.text = result.CurrentCoin.ToString();
            //     gemTxt.text = result.CurrentGem.ToString();
            //     PopupHelpers.ShowError("Buy successfully", "Congratulation");
            // }
        }

        public void CloseConfirm()
        {
            confirmBuy.SetActive(false);
        }

        public void OnClickClose()
        {
            ClosePopup();
        }
    }
}
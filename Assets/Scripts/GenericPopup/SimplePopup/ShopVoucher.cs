using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
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
        [SerializeField] private Button voucher1Btn;

        [SerializeField] private TextMeshProUGUI voucher2TxtPrice;
        [SerializeField] private TextMeshProUGUI voucher2CountLeft;
        [SerializeField] private Button voucher2Btn;

        [SerializeField] private TextMeshProUGUI voucher3TxtPrice;
        [SerializeField] private TextMeshProUGUI voucher3CountLeft;
        [SerializeField] private Button voucher3Btn;

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
            audioService = GameServices.Instance.GetService<AudioService>();
            var param = PopupHelpers.PassParamPopup();
            clientService.OnFailed = err =>
            {
                ActiveSafePanel(false);
                PopupHelpers.ShowError(err, "ERROR");
            };
        }

        private void Start()
        {
            audioService.PlaySound(GUISound.Popup);
            coinTxt.text = clientService.Coin.ToString();
            gemTxt.text = clientService.Gem.ToString();

            voucher1TxtPrice.text = priceDictionary[1].ToString();
            voucher2TxtPrice.text = priceDictionary[2].ToString();
            voucher3TxtPrice.text = priceDictionary[3].ToString();

            LoadVoucherCountLeft();
            animator.gameObject.SetActive(true);
        }

        private void LoadVoucherCountLeft()
        {
            if (clientService.IsLogin)
            {
                var value1 = playerService.LoadVoucherBoughtLeft(clientService.UserId, 1);
                var value2 = playerService.LoadVoucherBoughtLeft(clientService.UserId, 2);
                var value3 = playerService.LoadVoucherBoughtLeft(clientService.UserId, 3);
                voucher1CountLeft.text = value1 + " Left";
                voucher2CountLeft.text = value2 + " Left";
                voucher3CountLeft.text = value3 + " Left";

                if (value1 == 0)
                {
                    voucher1CountLeft.text = "Out of stock";
                    voucher1Btn.interactable = false;
                }

                if (value2 == 0)
                {
                    voucher1CountLeft.text = "Out of stock";
                    voucher2Btn.interactable = false;
                }

                if (value3 == 0)
                {
                    voucher1CountLeft.text = "Out of stock";
                    voucher3Btn.interactable = false;
                }
            }
        }

        private int tempIndex;

        public void OnClickBuy(int index)
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
            audioService.PlaySound(GUISound.Popup);
            detailConfirmBuy.text = "Are you sure want to buy this item for " + priceDictionary[index] + " Gems?";
            tempIndex = index;
            confirmBuy.SetActive(true);
        }

        public void ConfirmBuy()
        {
            ActiveSafePanel(true);
            var itemId = tempIndex;
            audioService.PlaySound(GUISound.Click);
            if (voucherIndex.Contains(tempIndex))
            {
                // Buy voucher
                clientService.BuyVoucher(itemId, priceDictionary[itemId], s =>
                {
                    coinTxt.text = clientService.Coin.ToString();
                    gemTxt.text = clientService.Gem.ToString();
                    playerService.SaveVoucherBought(clientService.UserId, itemId);
                    LoadVoucherCountLeft();
                    audioService.PlaySound(GUISound.Success);
                    audioService.PlaySound(GUISound.Popup);
                    PopupHelpers.ShowError("Buy Success, Thank for your purchase", "Notification");
                }, e => { PopupHelpers.ShowError(e); });
            }
            else
            {
                audioService.PlaySound(GUISound.Fail);
                PopupHelpers.ShowError("Ermm, This function not maintain yet. So sorry :(");
            }

            ActiveSafePanel(false);
            
        }

        public void CloseConfirm()
        {
            audioService.PlaySound(GUISound.Click);
            confirmBuy.SetActive(false);
        }

        public void OnClickClose()
        {
            audioService.PlaySound(GUISound.Click);
            ClosePopup();
        }
    }
}
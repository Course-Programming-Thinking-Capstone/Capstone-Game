using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace GenericPopup
{
    public class WinPopup : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI header;
        [SerializeField] private TextMeshProUGUI coinTxt;
        [SerializeField] private TextMeshProUGUI gemTxt;

        [SerializeField] private Button claimBtn;
        [SerializeField] private Button claimDoubleBtn;
        [SerializeField] private Button exitBtn;
        [SerializeField] private List<GameObject> starDisplay;

        [SerializeField] private Transform rewardCoin;
        [SerializeField] private Transform rewardGem;
        [SerializeField] private Transform headTf;
        [SerializeField] private Transform exit;
        [SerializeField] private Transform button1;
        [SerializeField] private Transform button2;
        [SerializeField] private Transform reward;

        [SerializeField] private float timeAppear = 1f;

        private void AppearAnimation()
        {
            // header
            var position = headTf.position;
            var originPos = position;
            position += Vector3.up * 100f;
            headTf.position = position;
            headTf.DOMove(originPos, timeAppear);

            // Exit
            position = exit.position;
            originPos = position;
            position += Vector3.right * 100f;

            exit.position = position;
            exit.DOMove(originPos, timeAppear);

            // buttons

            position = button1.position;
            originPos = position;
            position += Vector3.down * 100f;

            button1.position = position;
            button1.DOMove(originPos, timeAppear);

            position = button2.position;
            originPos = position;
            position += Vector3.down * 100f;

            button2.position = position;
            button2.DOMove(originPos, timeAppear);

            // Reward
            reward.localScale = Vector3.zero;
            reward.DOScale(Vector3.one, timeAppear);
        }

        private void Start()
        {
            var parameter = PopupHelpers.PassParamPopup();

            AppearAnimation();
            var callBack = parameter.GetAction(ActionType.YesOption);
            var callBackDouble = parameter.GetAction(ActionType.AdsOption);
            var callBackQuit = parameter.GetAction(ActionType.QuitOption);

            claimBtn.onClick.AddListener(callBack);
            claimDoubleBtn.onClick.AddListener(callBack);
            exitBtn.onClick.AddListener(callBackQuit);

            claimBtn.onClick.AddListener(PopupHelpers.Close);
            claimDoubleBtn.onClick.AddListener(PopupHelpers.Close);
            exitBtn.onClick.AddListener(PopupHelpers.Close);

            header.text = parameter.GetObject<string>("Title");
            var numOfStar = parameter.GetObject<int>("NumberOfStars");
            var numOfCoin = parameter.GetObject<int>("Coin");
            var numOfGem = parameter.GetObject<int>("Gem");

            if (numOfCoin == 0)
            {
                rewardCoin.gameObject.SetActive(false);
            }
            else
            {
                coinTxt.text = numOfCoin.ToString();
            }

            if (numOfGem == 0)
            {
                rewardGem.gameObject.SetActive(false);
            }
            else
            {
                gemTxt.text = numOfGem.ToString();
            }

            starDisplay[numOfStar - 1].SetActive(true);
        }
    }
}
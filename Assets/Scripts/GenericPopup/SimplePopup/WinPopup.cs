using System;
using System.Collections;
using JetBrains.Annotations;
using Services;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utilities;

namespace GenericPopup.SimplePopup
{
    public class WinPopup : PopupAdditive
    {
        [Header("Special")]
        [SerializeField] private TextMeshProUGUI coinWinTxt;
        [SerializeField] private Button nextLevelButton;
        [SerializeField] private Button homeButton;
        [Header("fruit")]
        [SerializeField] private Image renderItem;
        [SerializeField] private TextMeshProUGUI itemName;
        [SerializeField] private GameObject fruitObj;
        private UnityAction onClickNextLevelCallBack;
        [SerializeField] private string resourcesPath = "Fruits/";

        private void Awake()
        {
            audioService = GameServices.Instance.GetService<AudioService>();
        }

        private void Start()
        {
            var parameter = PopupHelpers.PassParamPopup();

            audioService.PlaySound(SoundToPlay.Won);
            onClickNextLevelCallBack = parameter.GetAction(PopupKey.YesOption);
            nextLevelButton.onClick.AddListener(OnClickNextLevel);
            homeButton.onClick.AddListener(OnClickHome);

            coinWinTxt.text = parameter.GetObject<int>(ParamType.CoinTxt).ToString();
            SetEarnItem(
                parameter.GetObject<string>(ParamType.WinItemUrl.ToString())
                , parameter.GetObject<string>(ParamType.WinItemName.ToString()
                ));
        }

        private void SetEarnItem([CanBeNull] string spritesUrl, [CanBeNull] string itemWinName)
        {
            if (string.IsNullOrEmpty(spritesUrl) || string.IsNullOrEmpty(itemWinName))
            {
                fruitObj.SetActive(false);
            }
            else
            {
                var sprites = Resources.Load<Sprite>(resourcesPath + spritesUrl);
                if (sprites != null)
                {
                    renderItem.sprite = sprites;
                }

                itemName.text = itemWinName;
            }
        }

        private void OnClickHome()
        {
            audioService.PlaySound(SoundToPlay.Click);
            if (animator != null)
            {
                animator.SetBool(exit, true);
                StartCoroutine(LoadMain(0.5f));
            }
            else
            {
                SceneManager.LoadScene(Constants.MainMenu);
            }
        }

        private void OnClickNextLevel()
        {
            audioService.PlaySound(SoundToPlay.Click);
            if (animator != null)
            {
                animator.SetBool(exit, true);
            }
            else
            {
                PopupHelpers.Close(gameObject.scene);
            }

            onClickNextLevelCallBack?.Invoke();
        }

        private IEnumerator LoadMain(float delay)
        {
            audioService.PlaySound(SoundToPlay.Click);
            yield return new WaitForSeconds(delay);
            SceneManager.LoadScene(Constants.MainMenu);
        }
    }
}
using System.Collections;
using JetBrains.Annotations;

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

        private void Start()
        {
            var parameter = PopupHelpers.PassParamPopup();

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
            if (spritesUrl == null || itemWinName == null)
            {
                fruitObj.SetActive(false);
            }
            else
            {
                var sprites = Resources.Load<Sprite>(spritesUrl);
                if (sprites != null)
                {
                    renderItem.sprite = sprites;
                }

                itemName.text = itemWinName;
            }
        }

        private void OnClickHome()
        {
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
            yield return new WaitForSeconds(delay);
            SceneManager.LoadScene(Constants.MainMenu);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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
        
        private UnityAction onClickNextLevelCallBack;

        private void Start()
        {
            var parameter = PopupHelpers.PassParamPopup();

            onClickNextLevelCallBack = parameter.GetAction(PopupKey.YesOption);
            nextLevelButton.onClick.AddListener(OnClickNextLevel);
            homeButton.onClick.AddListener(OnClickHome);
            coinWinTxt.text = parameter.GetObject<int>(ParamType.CoinTxt).ToString();
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
                PopupHelpers.Close(gameObject.scene);
            }
        }

        private void OnClickNextLevel()
        {
            if (animator != null)
            {
                animator.SetBool(exit, true);
                onClickNextLevelCallBack?.Invoke();
            }
            else
            {
                PopupHelpers.Close(gameObject.scene);
            }
        }

        private IEnumerator LoadMain(float delay)
        {
            yield return new WaitForSeconds(delay);
            SceneManager.LoadScene(Constants.MainMenu);
        }
    }
}
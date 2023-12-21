using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Utilities;

namespace GenericPopup
{
    public class LoginPopup : MonoBehaviour
    {
        [SerializeField] private InputField user;
        [SerializeField] private InputField password;

        [SerializeField] private Button login;
        [SerializeField] private Button closeButton;
        [SerializeField] private Button forgetPassword;
        [SerializeField] private Button signUp;

        private void Start()
        {
            var parameter = PopupHelpers.PassParamPopup();

            if (parameter == null)
            {
                PopupHelpers.Close();
            }

            var callBack = parameter.GetObject<UnityAction<string, string>>("Login");

            login.onClick.AddListener(() => { callBack.Invoke(user.text, password.text); });
            login.onClick.AddListener(PopupHelpers.Close);
            closeButton.onClick.AddListener(PopupHelpers.Close);
        }
    }
}
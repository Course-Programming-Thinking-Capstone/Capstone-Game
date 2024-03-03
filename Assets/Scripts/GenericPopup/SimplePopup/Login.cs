using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace GenericPopup.SimplePopup
{
    public class Login : PopupAdditive
    {
        [SerializeField] private TMP_InputField user;
        [SerializeField] private TMP_InputField password;

        [SerializeField] private Button login;
        [SerializeField] private Button closeButton;
        [SerializeField] private Button signUp;

        private void Start()
        {
            var parameter = PopupHelpers.PassParamPopup();

            if (parameter == null)
            {
                ClosePopup();
            }

            login.onClick.AddListener(OnClickLogin);
            signUp.onClick.AddListener(OnClickSigning);
            closeButton.onClick.AddListener(ClosePopup);
        }

        private void OnClickLogin()
        {
        }

        private void OnClickSigning()
        {
        }
    }
}
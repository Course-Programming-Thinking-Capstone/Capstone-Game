using Services;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
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

        private ClientService clientService;
        private UnityAction onLogin;

        private void Awake()
        {
            clientService = GameServices.Instance.GetService<ClientService>();
            clientService.OnFailed = err =>
            {
                ActiveSafePanel(false);
                PopupHelpers.ShowError(err, "ERROR");
            };
        }

        private void Start()
        {
            var parameter = PopupHelpers.PassParamPopup();

            if (parameter != null)
            {
                onLogin = parameter.GetAction(PopupKey.CallBack);
            }

            login.onClick.AddListener(OnClickLogin);
            signUp.onClick.AddListener(OnClickSigning);
            closeButton.onClick.AddListener(ClosePopup);
        }

        private void OnClickLogin()
        {
            ActiveSafePanel(true);
            clientService.LoginWithEmail(user.text, password.text,
                () =>
                {
                    onLogin?.Invoke();
                    ActiveSafePanel(false);
                    ClosePopup();
                    PopupHelpers.ShowError("Login successfully", "Notification");
                });
        }

        private void OnClickSigning()
        {
            Application.OpenURL("https://www.facebook.com/DenkTieu/");
        }
    }
}
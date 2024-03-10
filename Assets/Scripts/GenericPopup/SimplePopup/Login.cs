
using Services;
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

        private ServerSideService serverSideService;

        private void Awake()
        {
            serverSideService = GameServices.Instance.GetService<ServerSideService>();
            serverSideService.OnFailed = err => { PopupHelpers.ShowError(err, "ERROR"); };
        }

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
            serverSideService.LoginWithEmail(user.text, password.text,
                () => { PopupHelpers.ShowError("Login thành công", "Thông báo"); });
        }

        private void OnClickSigning()
        {
        }
    }
}
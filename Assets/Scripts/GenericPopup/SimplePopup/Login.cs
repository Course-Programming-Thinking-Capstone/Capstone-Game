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
        private GameService gameService;
        private UnityAction onLogin;

        private void Awake()
        {
            audioService = GameServices.Instance.GetService<AudioService>();

            clientService = GameServices.Instance.GetService<ClientService>();
            gameService = GameServices.Instance.GetService<GameService>();
            clientService.OnFailed = err =>
            {
                ActiveSafePanel(false);
                PopupHelpers.ShowError(err, "ERROR");
            };
        }

        private void Start()
        {
            audioService.PlaySound(SoundToPlay.Popup);
            var parameter = PopupHelpers.PassParamPopup();
            onLogin = parameter.GetAction(PopupKey.CallBack);

            login.onClick.AddListener(OnClickLogin);
            signUp.onClick.AddListener(OnClickSigning);
            closeButton.onClick.AddListener(ClosePopup);
        }

        private void OnClickLogin()
        {
            audioService.PlaySound(SoundToPlay.Click);
            ActiveSafePanel(true);
            clientService.LoginWithEmail(user.text, password.text,
                () =>
                {
                    audioService.PlaySound(SoundToPlay.Success);
                    onLogin?.Invoke();
                    ActiveSafePanel(false);
                    ClosePopup();
                    PopupHelpers.ShowError("Login successfully", "Notification");
                });
        }
        protected override void ClosePopup()
        {
            audioService.PlaySound(SoundToPlay.Click);
            base.ClosePopup();
        }
        private void OnClickSigning()
        {
            audioService.PlaySound(SoundToPlay.Click);
            gameService.Web();
        }
    }
}
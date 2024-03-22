using Services;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Utilities;

namespace GenericPopup.SimplePopup
{
    public class Setting : PopupAdditive
    {
        [SerializeField] private Button closeButton;
        [SerializeField] private Button webButton;
        [SerializeField] private Button facebookButton;
        [SerializeField] private Button logoutButton;
        [SerializeField] private Button languageButton;

        [SerializeField] private Slider sound;
        [SerializeField] private Slider music;

        private ClientService clientService;
        private UnityAction onLogout;

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

            if (parameter == null)
            {
                onLogout = parameter.GetAction(PopupKey.CallBack);
                Destroy(parameter);
                ClosePopup();
            }

            closeButton.onClick.AddListener(ClosePopup);
            logoutButton.onClick.AddListener(OnClickLogout);
            facebookButton.onClick.AddListener(OnClickFB);
            webButton.onClick.AddListener(OnClickWebsite);
        }

        private void OnClickLogout()
        {
            clientService.LogOut();
            ClosePopup();
            onLogout?.Invoke();
        }

        private void OnClickFB()
        {
            GameServices.Instance.GetService<GameService>().FaceBook();
        }

        private void OnClickWebsite()
        {
            GameServices.Instance.GetService<GameService>().Web();
        }
    }
}
using Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace GenericPopup.SimplePopup
{
    public class Profile : PopupAdditive
    {
        [SerializeField] private TMP_InputField userEmail;
        [SerializeField] private TMP_InputField userDisplayName;

        [SerializeField] private Button updateProfile;
        [SerializeField] private Button closeButton;

        private ClientService clientService;


        private void Awake()
        {
            clientService = GameServices.Instance.GetService<ClientService>();
            audioService = GameServices.Instance.GetService<AudioService>();
            clientService.OnFailed = err =>
            {
                ActiveSafePanel(false);
                PopupHelpers.ShowError(err, "ERROR");
            };
        }

        private void Start()
        {
            audioService.PlaySound(SoundToPlay.Popup);
            userEmail.text = clientService.UserEmail;
            userDisplayName.text = clientService.UserDisplayName;
            updateProfile.onClick.AddListener(OnClickUpdateProfile);
            closeButton.onClick.AddListener(ClosePopup);
        }

        protected override void ClosePopup()
        {
            audioService.PlaySound(SoundToPlay.Click);
            base.ClosePopup();
        }

        private void OnClickUpdateProfile()
        {
            audioService.PlaySound(SoundToPlay.Click);
        }
    }
}
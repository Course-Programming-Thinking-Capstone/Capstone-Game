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
        private AudioService audioService;
        private UnityAction onLogout;

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
            sound.value = audioService.SoundVolume;
            music.value = audioService.MusicVolume;

            sound.onValueChanged.AddListener(o => { audioService.SoundVolume = o; });
            music.onValueChanged.AddListener(o => { audioService.MusicVolume = o; });
            audioService.PlaySound(SoundToPlay.Popup);
            var parameter = PopupHelpers.PassParamPopup();

            if (parameter != null)
            {
                onLogout = parameter.GetAction(PopupKey.CallBack);
            }

            closeButton.onClick.AddListener(ClosePopup);
            logoutButton.onClick.AddListener(OnClickLogout);
            facebookButton.onClick.AddListener(OnClickFB);
            webButton.onClick.AddListener(OnClickWebsite);
            if (clientService.UserId == -1)
            {
                logoutButton.interactable = false;
            }
        }

        private void OnClickLogout()
        {
            audioService.PlaySound(SoundToPlay.Click);
            clientService.LogOut();
            ClosePopup();
            onLogout?.Invoke();
        }

        protected override void ClosePopup()
        {
            audioService.SaveSoundAndMusicVolume();
            audioService.PlaySound(SoundToPlay.Click);
            base.ClosePopup();
        }

        private void OnClickFB()
        {
            audioService.PlaySound(SoundToPlay.Click);
            GameServices.Instance.GetService<GameService>().FaceBook();
        }

        private void OnClickWebsite()
        {
            audioService.PlaySound(SoundToPlay.Click);
            GameServices.Instance.GetService<GameService>().Web();
        }
    }
}
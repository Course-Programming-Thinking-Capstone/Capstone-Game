using Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace GenericPopup.SimplePopup
{
    public class Error : PopupAdditive
    {
        [SerializeField] private TextMeshProUGUI header;
        [SerializeField] private TextMeshProUGUI detail;
        [SerializeField] private Button okButton;

        private void Start()
        {
            audioService = GameServices.Instance.GetService<AudioService>();
            var parameter = PopupHelpers.PassParamPopup();

            if (parameter == null)
            {
                ClosePopup();
            }

            okButton.onClick.AddListener(() =>
            {
                audioService.PlaySound(SoundToPlay.Click);
                ClosePopup();
            });

            header.text = parameter.GetObject<string>(PopupKey.IsErrorKey.ToString());
            detail.text = parameter.GetObject<string>(PopupKey.DescriptionKey.ToString());
        }
    }
}
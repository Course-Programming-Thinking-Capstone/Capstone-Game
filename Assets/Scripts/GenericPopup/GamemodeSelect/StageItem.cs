using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Utilities;

namespace GenericPopup.GameModeSelect
{
    public class StageItem : MonoBehaviour
    {
        [SerializeField] private GameObject locker;
        [SerializeField] private Button button;
        [SerializeField] private Image render;
        [SerializeField] private TextMeshProUGUI label;

        public void Initialized([CanBeNull] Sprite displayImage, string detail, UnityAction callBack, bool isLocked)
        {
            if (displayImage != null)
            {
                render.sprite = displayImage;
            }

            label.text = detail;
            if (isLocked)
            {
                button.onClick.AddListener(() =>
                {
                    PopupHelpers.ShowError(
                        "Please play at least " + Constants.FreeLevel + " previous levels to unlock this mode",
                        "Notification");
                });
            }
            else
            {
                button.onClick.AddListener(callBack);
            }

            render.SetNativeSize();
            locker.SetActive(isLocked);
        }
    }
}
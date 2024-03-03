using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GenericPopup.GameModeSelect
{
    public class StageItem : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private Image render;
        [SerializeField] private TextMeshProUGUI label;

        public void Initialized([CanBeNull] Sprite displayImage, string detail, UnityAction callBack)
        {
            if (displayImage != null)
            {
                render.sprite = displayImage;
            }

            label.text = detail;
            button.onClick.AddListener(callBack);
            render.SetNativeSize();
        }
    }
}
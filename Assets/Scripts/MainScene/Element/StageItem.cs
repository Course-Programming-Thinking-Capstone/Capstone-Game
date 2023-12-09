using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MainScene.Element
{
    public class StageItem : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private Image render;
        [SerializeField] private TextMeshProUGUI label;
    

        public void Initialized(Sprite displayImage, string detail, UnityAction callBack)
        {
            render.sprite = displayImage;
            label.text = detail;
            button.onClick.AddListener(callBack);
            render.SetNativeSize();
        }
    }
}
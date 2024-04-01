using AYellowpaper.SerializedCollections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GenericPopup.Inventory
{
    public class InventItem : MonoBehaviour
    {
        [SerializeField]
        [SerializedDictionary("RateType", "Sprite color")]
        private SerializedDictionary<RateType, Sprite> rateRender;
        [SerializeField] private Image renderImg;
        [SerializeField] private Image rateRenderImg;
        [SerializeField] private TextMeshProUGUI quantityTxt;
        [SerializeField] private GameObject focusObj;

        private UnityAction onClick;

        public void InitializedItem(RateType rateType, Sprite sprite, int quantity, UnityAction callBack)
        {
            renderImg.sprite = sprite;
            quantityTxt.text = quantity.ToString();
            onClick = callBack;
            rateRenderImg.sprite = rateRender[rateType];
        }

        public void OnClickThisItem()
        {
            SetFocus(true);
            onClick?.Invoke();
        }

        public void SetFocus(bool isActive)
        {
            focusObj.SetActive(isActive);
        }
    }

    public enum RateType
    {
        Orange,
        Red,
        Purple,
        Blue,
        Green,
        Gray
    }
}
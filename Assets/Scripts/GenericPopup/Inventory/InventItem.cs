using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GenericPopup.Inventory
{
    public class InventItem : MonoBehaviour
    {
    
        [SerializeField] private Image renderImg;
        [SerializeField] private Image rateRenderImg;
        [SerializeField] private TextMeshProUGUI quantityTxt;
        [SerializeField] private GameObject focusObj;

        private UnityAction onClick;

        public void InitializedItem(Sprite rateTypeSprite, Sprite sprite, int quantity, UnityAction callBack)
        {
            renderImg.sprite = sprite;
            quantityTxt.text = quantity.ToString();
            onClick = callBack;
            rateRenderImg.sprite = rateTypeSprite;
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
}
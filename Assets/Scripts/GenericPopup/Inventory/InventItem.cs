using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GenericPopup.Inventory
{
    public class InventItem : MonoBehaviour
    {
        [SerializeField]
        private List<Sprite> rateRender;
        [SerializeField] private Image renderImg;
        [SerializeField] private Image rateRenderImg;
        [SerializeField] private TextMeshProUGUI quantityTxt;
        private UnityAction onClick;

        public void InitializedItem(Sprite sprite, int quantity, UnityAction callBack)
        {
            renderImg.sprite = sprite;
            quantityTxt.text = quantity.ToString();
            onClick = callBack;
        }

        public void OnClickThisItem()
        {
            onClick?.Invoke();
        }
    }

    public enum ValueRate
    {
        Orange,
        Red,
        Purple,
        Blue,
        Green,
        Gray
    }
}
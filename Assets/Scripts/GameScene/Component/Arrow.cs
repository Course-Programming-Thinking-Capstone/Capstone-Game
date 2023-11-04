using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameScene.Component
{
    public class Arrow : Selector, IPointerDownHandler
    {
        public float Size { get; set; }
        private void Awake()
        {
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            OnClickButton();
        }
    }
}
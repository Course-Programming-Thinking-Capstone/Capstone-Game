using UnityEngine;
using UnityEngine.EventSystems;

namespace GameScene.Component
{
    public class Arrow : Selector, IPointerDownHandler
    {
        public void OnPointerDown(PointerEventData eventData)
        {
  
            OnClickButton();
        }
    }
}
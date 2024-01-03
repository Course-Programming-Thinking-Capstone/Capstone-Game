using UnityEngine;
using UnityEngine.EventSystems;

namespace GameScene.Component.GameBasic
{
    public class Road : Selector, IPointerDownHandler
    {

        public void OnPointerDown(PointerEventData eventData)
        {
            OnClickButton();
        }
    }
}
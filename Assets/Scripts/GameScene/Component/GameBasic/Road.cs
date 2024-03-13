using UnityEngine;
using UnityEngine.EventSystems;

namespace GameScene.Component.GameBasic
{
    public class Road : InteractionItem, IPointerDownHandler
    {

        public void OnPointerDown(PointerEventData eventData)
        {
            OnClickButton();
        }
    }
}
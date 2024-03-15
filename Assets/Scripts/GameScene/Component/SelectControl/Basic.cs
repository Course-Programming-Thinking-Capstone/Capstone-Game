using UnityEngine.EventSystems;

namespace GameScene.Component.SelectControl
{
    public class Basic : InteractionItem, IPointerDownHandler
    {
        public void OnPointerDown(PointerEventData eventData)
        {
            OnClickButton();
        }

    }
}
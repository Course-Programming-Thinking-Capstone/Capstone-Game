using UnityEngine.EventSystems;

namespace GameScene.Component.SelectControl
{
    public class Arrow : InteractionItem, IPointerDownHandler
    {
        public void OnPointerDown(PointerEventData eventData)
        {
            OnClickButton();
        }

    }
}
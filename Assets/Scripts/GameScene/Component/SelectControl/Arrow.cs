using UnityEngine.EventSystems;

namespace GameScene.Component.SelectControl
{
    public class Arrow : Selector, IPointerDownHandler
    {
        public void OnPointerDown(PointerEventData eventData)
        {
            OnClickButton();
        }

        public override void ActiveEffect(bool isActive = true)
        {
            base.ActiveEffect(isActive);
        }
    }
}
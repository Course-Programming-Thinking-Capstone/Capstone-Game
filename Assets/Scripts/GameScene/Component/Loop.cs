using UnityEngine;
using UnityEngine.EventSystems;

namespace GameScene.Component
{
    public class Loop : Selector, IPointerDownHandler
    {
        public void OnPointerDown(PointerEventData eventData)
        {
            OnClickButton();
        }

        public override void ChangeRender(Sprite newRender)
        {
            base.ChangeRender(newRender);
            renderer.SetNativeSize();
        }
    }
}
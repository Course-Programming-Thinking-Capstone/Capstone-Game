using UnityEngine.EventSystems;
using UnityEngine.UI;

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
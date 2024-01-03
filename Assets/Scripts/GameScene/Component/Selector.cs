using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameScene.Component
{
    public class Selector : MonoBehaviour
    {
        [SerializeField] private SelectType selectType;
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private Image renderer;

        public void ChangeRender(Sprite newRender)
        {
            renderer.sprite = newRender;
        }

        public SelectType SelectType => selectType;
        public RectTransform RectTransform => rectTransform;
        private UnityAction<Selector> onClick;

        /// <summary>
        /// Call Init when instantiate object 
        /// </summary>
        public void Init(UnityAction<Selector> onClickParam)
        {
            rectTransform = GetComponent<RectTransform>();
            if (renderer == null)
            {
                renderer = gameObject.GetComponent<Image>();
            }
            onClick = onClickParam;
        }

        /// <summary>
        /// On Click this object
        /// </summary>
        public void OnClickButton()
        {
            onClick?.Invoke(this);
        }
    }
}
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameScene.Component
{
    public class Selector : MonoBehaviour
    {
        [SerializeField] protected SelectType selectType;
        [SerializeField] protected RectTransform rectTransform;
        [SerializeField] protected Image renderer;
        [SerializeField] protected GameObject effectRender;

        public float GetHeight()
        {
            return rectTransform.sizeDelta.y;
        }

        public virtual void ChangeRender(Sprite newRender)
        {
            renderer.sprite = newRender;
        }

        public SelectType SelectType
        {
            get => selectType;
            set => selectType = value;
        }
        public RectTransform RectTransform => rectTransform;
        private UnityAction<Selector> onClick;

        /// <summary>
        /// Call Init when instantiate object 
        /// </summary>
        public virtual void Init(UnityAction<Selector> onClickParam)
        {
            rectTransform = GetComponent<RectTransform>();
            if (renderer == null)
            {
                renderer = gameObject.GetComponent<Image>();
            }

            onClick = onClickParam;
        }

        public virtual void ActiveEffect(bool isActive = true)
        {
            effectRender.SetActive(isActive);
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
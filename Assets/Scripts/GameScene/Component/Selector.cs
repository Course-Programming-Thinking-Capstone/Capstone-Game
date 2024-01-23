using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameScene.Component
{
    public class Selector : MonoBehaviour
    {
        [SerializeField] protected SelectType selectType;
        [SerializeField] protected RectTransform rectTransform;
        [SerializeField] protected Image rendererImg;
        [SerializeField] protected GameObject effectRender;

        public float GetHeight()
        {
            return rectTransform.sizeDelta.y;
        }

        public virtual void ChangeRender(Sprite newRender)
        {
            rendererImg.sprite = newRender;
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
            if (rendererImg == null)
            {
                rendererImg = gameObject.GetComponent<Image>();
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
using UnityEngine;
using UnityEngine.Events;

namespace GameScene.Component
{
    public class Selector: MonoBehaviour
    {
        [SerializeField] private SelectType selectType;
        [SerializeField] private RectTransform rectTransform;
        public SelectType SelectType => selectType;
        public RectTransform RectTransform => rectTransform;
        private UnityAction<Selector> onClick;

        /// <summary>
        /// Call Init when instantiate object 
        /// </summary>
        public void Init(UnityAction<Selector> onClickParam)
        {
            rectTransform = GetComponent<RectTransform>();
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
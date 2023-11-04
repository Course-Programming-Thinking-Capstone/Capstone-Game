using UnityEngine;
using UnityEngine.Events;

namespace GameScene.Component
{
    public class Selector: MonoBehaviour
    {
        [SerializeField] private SelectType selectType;
        public SelectType SelectType => selectType;
        private UnityAction<Selector> onClick;

        /// <summary>
        /// Call Init when instantiate object 
        /// </summary>
        public void Init(UnityAction<Selector> onClickParam)
        {
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
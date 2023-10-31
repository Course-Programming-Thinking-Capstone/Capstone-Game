using UnityEngine;
using UnityEngine.Events;

namespace GameScene.Component
{
    public class Selector: MonoBehaviour
    {
        [SerializeField] private SelectType selectType;
        private SelectType SelectType => selectType;
        private UnityAction<SelectType> onClick;

        /// <summary>
        /// Call Init when instantiate object 
        /// </summary>
        public void Init(UnityAction<SelectType> onClickParam)
        {
            onClick = onClickParam;
        }

        /// <summary>
        /// On Click this object
        /// </summary>
        public void OnClickButton()
        {
            onClick?.Invoke(SelectType);
        }
    }
}
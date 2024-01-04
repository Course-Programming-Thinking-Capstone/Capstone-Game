using UnityEngine;
using UnityEngine.Events;

namespace GameScene.Component.GameBasic
{
    public class GroundRoad : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer rendererGround;
        [SerializeField] private SelectType selectType = SelectType.None;
         private UnityAction<SelectType> callBack;

        public void ChangeRender(Sprite newSprite, SelectType type)
        {
            rendererGround.sprite = newSprite;
            selectType = type;
        }

        public void OnClick()
        {
            callBack.Invoke(selectType);
        }
    }
}
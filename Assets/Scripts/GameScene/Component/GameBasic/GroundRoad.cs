using UnityEngine;

namespace GameScene.Component.GameBasic
{
    public class GroundRoad : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer rendererGround;

        public void ChangeRender(Sprite newSprite)
        {
            rendererGround.sprite = newSprite;
        }
    }
}
using UnityEngine;
using UnityEngine.UI;

namespace GameScene.Component
{
    public class Candy : MonoBehaviour
    {
        public void Init(Sprite candySprite)
        {
            GetComponent<SpriteRenderer>().sprite = candySprite;
        }
    }
}
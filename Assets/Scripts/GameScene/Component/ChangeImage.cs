using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameScene.Component
{
    public class ChangeImage : MonoBehaviour
    {
        [SerializeField] private Image renderer;

        private void Awake()
        {
            if (renderer == null)
            {
                renderer = gameObject.GetComponent<Image>();
            }
        }

        public void ChangeRender(Sprite newRender)
        {
            renderer.sprite = newRender;
        }
    }
}
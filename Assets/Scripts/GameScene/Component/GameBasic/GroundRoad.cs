using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

namespace GameScene.Component.GameBasic
{
    public class GroundRoad : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer rendererGround;
        private UnityAction<GroundRoad> callBack;
        public InteractionItem CurrentDisplay { get; set; }

        public void Initialized(UnityAction<GroundRoad> callBackController)
        {
            callBack = callBackController;
        }

        public void ChangeRender(Sprite newSprite, [CanBeNull] InteractionItem type)
        {
            rendererGround.sprite = newSprite;
            CurrentDisplay = type;
        }

        private void OnMouseDown()
        {
            if (CurrentDisplay)
            {
                callBack.Invoke(this);
            }
        }
    }
}
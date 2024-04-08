using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace GameScene.Component.GameBasic
{
    public class GroundRoad : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer rendererGround;
        [SerializeField] private Sprite questSprite;
        [SerializeField] private Sprite baseSprite;
        private UnityAction<GroundRoad> callBack;
        public InteractionItem CurrentDisplay { get; set; }

        public void Initialized(UnityAction<GroundRoad> callBackController)
        {
            callBack = callBackController;
        }

        public void ChangeToQuestionRender()
        {
            rendererGround.sprite = questSprite;
            CurrentDisplay = null;
        }

        public void ChangeToBaseRender()
        {
            rendererGround.sprite = baseSprite;
            CurrentDisplay = null;
        }

        public void ChangeRender(Sprite newSprite, [CanBeNull] InteractionItem type)
        {
            rendererGround.sprite = newSprite;
            CurrentDisplay = type;
        }

        private void OnMouseDown()
        {
            if (!IsPointerOverUIObject())
            {
                if (CurrentDisplay)
                {
                    callBack.Invoke(this);
                }
            }
        }

        bool IsPointerOverUIObject()
        {
            EventSystem eventSystem = EventSystem.current;
            if (eventSystem == null)
                return false;

            return eventSystem.IsPointerOverGameObject();
        }
    }
}
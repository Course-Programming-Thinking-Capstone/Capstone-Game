using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace GameScene.Component
{
    public class Loop : Selector, IPointerDownHandler
    {
        [SerializeField] private int maxLoop = 8;
        [SerializeField] private TextMeshProUGUI loopTxt;
        public int LoopCount { get; set; } = 2;

        public override void Init(UnityAction<Selector> onClickParam)
        {
            base.Init(onClickParam);
            LoopCount = 2;
            loopTxt.text = LoopCount.ToString();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            OnClickButton();
        }

        public override void ChangeRender(Sprite newRender)
        {
            base.ChangeRender(newRender);
            renderer.SetNativeSize();
        }

        public void OnClickLoop()
        {
            if (LoopCount == maxLoop)
            {
                LoopCount = 2;
            }
            else
            {
                LoopCount++;
            }

            loopTxt.text = LoopCount.ToString();
        }
    }
}
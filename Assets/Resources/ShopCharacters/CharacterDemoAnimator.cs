using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace Resources.ShopCharacters
{
    public class CharacterDemoAnimator : MonoBehaviour
    {
        [SerializeField]
        private List<string> demoAnimation = new List<string>()
        {
            "action/idle/normal"
        };

        private SkeletonGraphic spineAnimation;
        private int currentDemo = 0;

        void Start()
        {
            gameObject.GetComponent<Button>().onClick.AddListener(OnClick);
            spineAnimation = gameObject.GetComponent<SkeletonGraphic>();
            spineAnimation.AnimationState
                .SetAnimation(0, demoAnimation[currentDemo], true);
        }

        public void OnClick()
        {
            if (spineAnimation == null)
            {
                spineAnimation = gameObject.GetComponent<SkeletonGraphic>();
            }

            currentDemo++;
            if (currentDemo >= demoAnimation.Count)
            {
                currentDemo = 0;
            }

            spineAnimation.AnimationState
                .SetAnimation(0, demoAnimation[currentDemo], true);
        }
    }
}
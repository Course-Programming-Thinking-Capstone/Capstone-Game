using Spine.Unity;
using UnityEngine;

namespace GameScene.Component
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private SkeletonAnimation skeletonAnimation;

        [SerializeField] private string idleAnimation;
        [SerializeField] private string moveAnimation;
        [SerializeField] private string collectAnimation;

        public void PlayAnimationMove()
        {
            skeletonAnimation.AnimationState.SetAnimation(0, moveAnimation, true);
        }
        public void PlayAnimationIdle()
        {
            skeletonAnimation.AnimationState.SetAnimation(0, idleAnimation, true);
        }
        
        public void PlayAnimationEat()
        {
            skeletonAnimation.AnimationState.SetAnimation(0, collectAnimation, true);
        }
    }
}

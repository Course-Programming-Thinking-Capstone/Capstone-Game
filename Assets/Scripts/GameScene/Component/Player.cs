using DG.Tweening;
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
        public void RotatePlayer(bool isRight, float timeToRotate)
        {
            var targetRotate = isRight ? new Vector3(0, 180, 0) : new Vector3(0, 0, 0);
            transform.DORotate(targetRotate, timeToRotate);
        }
    }
}

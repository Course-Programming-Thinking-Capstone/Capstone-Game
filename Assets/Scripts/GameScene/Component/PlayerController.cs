using System.Collections;
using DG.Tweening;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace GameScene.Component
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private SkeletonAnimation skeletonAnimation;

        [SerializeField] private string idleAnimation;
        [SerializeField] private string moveAnimation;
        [SerializeField] private string collectAnimation;

        public IEnumerator MovePlayer(Vector2 targetMove, float moveTime)
        {
            if (targetMove.x < transform.position.x)
            {
                RotatePlayer(false, moveTime);
            }
            else if (targetMove.x > transform.position.x)
            {
                RotatePlayer(true, moveTime);
            }

            var movePromise = transform.DOMove(targetMove, moveTime);
            PlayAnimationMove();
            yield return movePromise.WaitForCompletion();
        }

        public void ForceMovePlayer(Vector2 targetPos)
        {
            transform.position = targetPos;
        }

        public TrackEntry PlayAnimationMove()
        {
            return skeletonAnimation.AnimationState.SetAnimation(0, moveAnimation, true);
        }

        public TrackEntry PlayAnimationIdle()
        {
            return skeletonAnimation.AnimationState.SetAnimation(0, idleAnimation, false);
        }

        public TrackEntry PlayAnimationEat()
        {
            return skeletonAnimation.AnimationState.SetAnimation(0, collectAnimation, true);
        }

        public void RotatePlayer(bool isRight, float timeToRotate)
        {
            var targetRotate = isRight ? new Vector3(0, 180, 0) : new Vector3(0, 0, 0);
            transform.DORotate(targetRotate, timeToRotate);
        }
    }
}
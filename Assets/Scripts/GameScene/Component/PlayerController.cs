using System;
using System.Collections;
using System.Linq;
using DG.Tweening;
using Services;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace GameScene.Component
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private SkeletonAnimation skeletonAnimation;
        [SerializeField] private Transform skeletonContainer;
        [SerializeField] private GameObject defaultCharacter;

        [SerializeField] private string idleAnimation;
        [SerializeField] private string moveAnimation;
        [SerializeField] private string collectAnimation;

        private void Start()
        {
            var clientService = GameServices.Instance.GetService<ClientService>();
            var playerService = GameServices.Instance.GetService<PlayerService>();

            if (!clientService.IsLogin)
            {
                return;
            }

            // not login
            if (!clientService.IsLogin)
            {
                defaultCharacter.SetActive(true);
                return;
            }

            // not select new
            if (playerService.SelectedCharacter == -1)
            {
                defaultCharacter.SetActive(true);
                return;
            }

            // not have shop data
            if (clientService.CacheShopData == null)
            {
                return;
            }

            var character =
                clientService.CacheShopData.FirstOrDefault(o => o.Id == playerService.SelectedCharacter);
            defaultCharacter.SetActive(false);

            if (character != null)
            {
                var characterModel = Resources.Load<GameObject>("InGameCharacters/" + character.SpritesUrl);
                if (characterModel != null)
                {
                    InitPlayerModel(characterModel);
                }
            }
        }

        public void InitPlayerModel(GameObject spineModel)
        {
            var obj = Instantiate(spineModel, skeletonContainer);
            obj.transform.localScale = Vector3.one;
            obj.transform.rotation = Quaternion.identity;
            obj.transform.localPosition = Vector3.zero;
            skeletonAnimation = obj.GetComponent<SkeletonAnimation>();
            defaultCharacter.SetActive(false);
            PlayAnimationIdle(true);
        }

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

        public TrackEntry PlayAnimationIdle(bool isLoop = false)
        {
            return skeletonAnimation.AnimationState.SetAnimation(0, idleAnimation, isLoop);
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
using System.Collections;
using DG.Tweening;
using GameScene.Component;
using Services;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameScene
{
    public class GameController : MonoBehaviour
    {
        // SERVICES
        [SerializeField] private bool isTesting;
        protected PlayerService playerService;
        protected Player playerControl;

        // Param
        [SerializeField] protected Vector2 playerPosition;
        [SerializeField] protected Vector2 targetPosition;
        [SerializeField] protected Vector2 boardSize;
        protected IEnumerator MovePlayer(Vector2 targetMove, float moveTime)
        {
            if (targetMove.x < playerControl.transform.position.x)
            {
                playerControl.RotatePlayer(false, moveTime);
            }
            else if (targetMove.x > playerControl.transform.position.x)
            {
                playerControl.RotatePlayer(true, moveTime);
            }

            var movePromise = playerControl.transform.DOMove(targetMove, moveTime);
            playerControl.PlayAnimationMove();
            yield return movePromise.WaitForCompletion();
        }

        private void Awake()
        {
            if (isTesting)
            {
                return;
            }

            // Load services
            if (GameObject.FindGameObjectWithTag(Constants.ServicesTag) != null)
            {
                var services = GameObject.FindGameObjectWithTag(Constants.ServicesTag).GetComponent<GameServices>();
                playerService = services.GetService<PlayerService>();
            }
            else
            {
                SceneManager.LoadScene(Constants.EntryScene);
            }
        }
    }
}
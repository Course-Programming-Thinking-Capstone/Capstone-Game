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

        protected void RotatePlayer(bool isRight, float timeToRotate)
        {
            var targetRotate = isRight ? new Vector3(0, 180, 0) : new Vector3(0, 0, 0);
            playerControl.transform.DORotate(targetRotate, timeToRotate);
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
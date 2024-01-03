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
        // Param
        [SerializeField] protected Vector2 playerPosition;
        [SerializeField] protected Vector2 targetPosition;

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
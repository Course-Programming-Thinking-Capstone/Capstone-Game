using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameScene.Component;
using Services;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GameScene
{
    public class GameController : MonoBehaviour
    {
        // SERVICES
        [Header("All Mode")]
        [SerializeField] private bool isTesting;
        protected PlayerService playerService;
        protected Player playerControl;

        // Param
        [SerializeField] protected Vector2 playerPosition;
        [SerializeField] protected List<Vector2> targetPosition;
        [SerializeField] protected Vector2 boardSize;

        [Header("Drag mode")]
        [Header("Reference object game")]
        [SerializeField] protected RectTransform deleteZone;
        [SerializeField] protected RectTransform selectedZone;
        [SerializeField] protected Button playButton;
        [SerializeField] protected List<SelectType> generateList;
        
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
        
        
        protected bool IsPointInRT(Vector2 point, RectTransform rt)
        {
            // Get the rectangular bounding box of your UI element
            var rect = rt.rect;
            var anchoredPosition = rt.position;
            // Get the left, right, top, and bottom boundaries of the rect
            float leftSide = anchoredPosition.x - rect.width / 2f;
            float rightSide = anchoredPosition.x + rect.width / 2f;
            float topSide = anchoredPosition.y + rect.height / 2f;
            float bottomSide = anchoredPosition.y - rect.height / 2f;

            // Check to see if the point is in the calculated bounds
            if (point.x >= leftSide &&
                point.x <= rightSide &&
                point.y >= bottomSide &&
                point.y <= topSide)
            {
                return true;
            }

            return false;
        }

    }
}
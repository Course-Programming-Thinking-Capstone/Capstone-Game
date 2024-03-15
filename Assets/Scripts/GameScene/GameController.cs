using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameScene.Component;
using Services;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utilities;

namespace GameScene
{
    public class GameController : MonoBehaviour
    {
        // SERVICES
        [Header("All Mode")]
        [SerializeField] private bool isTesting;
        [SerializeField] protected GameMode gameMode;
        [SerializeField] protected PlayerController playerController;
        protected int levelIndex;

        // Param
        [SerializeField] protected Vector2 basePlayerPosition;
        [SerializeField] protected List<Vector2> targetPosition;
        [SerializeField] protected Vector2 boardSize = new(8, 6);

    

        protected void ShowWinPopup(int coinWin)
        {
            var parameter = PopupHelpers.PassParamPopup();
            parameter.AddAction(PopupKey.YesOption, OnLoadNextLevel);
            parameter.SaveObject(ParamType.CoinTxt, coinWin);
            PopupHelpers.Show(Constants.WinPopup);
        }

        private void OnLoadNextLevel()
        {
            var param = PopupHelpers.PassParamPopup();
            param.SaveObject(ParamType.LevelIndex, levelIndex);
            switch (gameMode)
            {
                case GameMode.Basic:
                    SceneManager.LoadScene(Constants.BasicMode);
                    break;
                case GameMode.Sequence:
                    SceneManager.LoadScene(Constants.SequenceMode);
                    break;
                case GameMode.Loop:
                    SceneManager.LoadScene(Constants.LoopMode);
                    break;
                case GameMode.Function:
                    SceneManager.LoadScene(Constants.FuncMode);
                    break;
                case GameMode.Condition:
                    SceneManager.LoadScene(Constants.ConditionMode);
                    break;
            }
        }

        public void OnClickExit()
        {
            SceneManager.LoadScene(Constants.MainMenu);
        }

        protected IEnumerator MovePlayer(Vector2 targetMove, float moveTime)
        {
            if (targetMove.x < playerController.transform.position.x)
            {
                playerController.RotatePlayer(false, moveTime);
            }
            else if (targetMove.x > playerController.transform.position.x)
            {
                playerController.RotatePlayer(true, moveTime);
            }

            var movePromise = playerController.transform.DOMove(targetMove, moveTime);
            playerController.PlayAnimationMove();
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
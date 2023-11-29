using System;
using Services;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MainScene
{
    public class MainSceneController : MonoBehaviour
    {
        [Header("MVC")]
        [SerializeField] private MainSceneModel model;
        [SerializeField] private MainSceneView view;

        [Header("SYSTEM")]
        private PlayerService playerService;

        private void Awake()
        {
            var objectService = GameObject.FindWithTag(Constants.ServicesTag);
            if (objectService == null)
            {
                SceneManager.LoadScene(Constants.EntryScene);
                return;
            }

            var gameServices = objectService.GetComponent<GameServices>();
            playerService = gameServices.GetService<PlayerService>();
        }

        private void Start()
        {
            // main 
            view.InitializedMain(OnClickPlay, OnClickShop, OnClickInventory, OnClickSetting);
            view.SetDisplayUserCoin(playerService.UserCoin);
            view.SetDisplayUserDiamond(playerService.UserDiamond);
            view.SetDisplayUserName("Denk");
            view.SetDisplayUserProcess("50 / 100", 0.5f);
            view.SetDisplayUserLevel(68);

            // popup
            view.InitializeLevel();
            view.InitializeStage();
        }

        private void OnClickSetting()
        {
            Debug.Log("Setting");
        }

        private void OnClickInventory()
        {
            Debug.Log("Invent");
        }

        private void OnClickShop()
        {
            Debug.Log("Shop");
        }

        private void OnClickPlay()
        {
            view.OpenStage(playerService.UserCoin, playerService.UserDiamond);
        }
    }
}
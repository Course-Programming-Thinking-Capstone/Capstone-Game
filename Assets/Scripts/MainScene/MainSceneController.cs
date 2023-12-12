using System;
using MainScene.Element;
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
            InitMain();

            // popup
            view.InitializeLevel();
            view.InitializeStage();

            // Create State
            InitStageLevel();
        }

        #region Initialized

        private void InitMain()
        {
            view.InitializedMain(OnClickPlay, OnClickShop, OnClickInventory, OnClickSetting);
            view.SetDisplayUserCoin(playerService.UserCoin);
            view.SetDisplayUserDiamond(playerService.UserDiamond);
            view.SetDisplayUserName("Denk");
            view.SetDisplayUserProcess("50 / 100", 0.5f);
            view.SetDisplayUserLevel(68);
        }

        private void InitStageLevel()
        {
            var modelStateObj = model.StageData.ModelPrefab;
            for (int i = 0; i < model.StageData.StageItemData.Count; i++)
            {
                var item = model.StageData.StageItemData[i];
                var obj = Instantiate(modelStateObj);
                var itemStage = obj.GetComponent<StageItem>();
                var i1 = i;
                itemStage.Initialized(item.render, item.detail, () => { OnClickStage(i1); });
                view.AddStageItem(obj.transform);
            }
        }

        #endregion

        #region CallBack

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

        private void OnClickStage(int index)
        {
            Debug.Log("Stage: " + index);
        }

        #endregion
    }
}
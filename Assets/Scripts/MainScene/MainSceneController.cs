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
            view.InitializeLevel(model.StageData.StageItemData.Count, OnClickHome);
            view.InitializeStage(OnClickHome);

            // Create State
            InitStage();
            InitLevel();
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

        private void InitStage()
        {
            var modelStateObj = model.StageData.ModelPrefab;
            for (int i = 0; i < model.StageData.StageItemData.Count; i++)
            {
                var item = model.StageData.StageItemData[i];
                var obj = Instantiate(modelStateObj);
                var itemStage = obj.GetComponent<StageItem>();
                var i1 = i;
                itemStage.Initialized(item.Render, item.Detail, () => { OnClickStage(i1); });
                view.AddStageItem(obj.transform);
            }
        }

        private void InitLevel()
        {
            view.InitializeLevel(model.StageData.StageItemData.Count, OnClickHome);
            for (int i = 0; i < model.StageData.StageItemData.Count; i++)
            {
                var listlevel = model.StageData.StageItemData[i].DataLevel;
                for (int j = 0; j < listlevel.LevelItemData.Count; j++)
                {
                    var modelStateObj = listlevel.ModelPrefab;
                    var obj = Instantiate(modelStateObj);
                    var itemStage = obj.GetComponent<LevelItem>();
                    var i1 = i;
                    itemStage.Initialized(null, null, null,
                        j, true, j == listlevel.LevelItemData.Count - 1, j == 0);
                    if (listlevel.LevelItemData[j].GemBonus != 0)
                    {
                        itemStage.SetActiveTop(true);
                    }

                    if (listlevel.LevelItemData[j].GoldBonus != 0)
                    {
                        itemStage.SetActiveTop(true);
                    }

                    view.AddLevelItem(obj.transform, j);
                }
            }
        }

        #endregion

        #region CallBack

        private void OnClickLevel(int stageIndex, int levelIndex)
        {
        }

        private void OnClickHome()
        {
            view.CloseAllPopup();
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

        private void OnClickStage(int index)
        {
            view.OpenLevel(playerService.UserCoin, playerService.UserDiamond, index);
        }

        #endregion
    }
}
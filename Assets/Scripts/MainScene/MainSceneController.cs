using System;
using System.Collections.Generic;
using System.Linq;
using MainScene.Element;
using Services;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;

namespace MainScene
{
    public class MainSceneController : MonoBehaviour
    {
        [Header("MVC")]
        [SerializeField] private MainSceneModel model;
        [SerializeField] private MainSceneView view;

        [Header("SYSTEM")]
        private PlayerService playerService;

        private List<int> currentLevel;

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
            currentLevel = playerService.CurrentLevel;
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
            for (int i = 0; i < model.StageData.StageItemData.Count; i++)
            {
                var historyLevel = playerService.GetHistoryStar(i);
                var listlevel = model.StageData.StageItemData[i].DataLevel;
                for (int j = 0; j < listlevel.LevelItemData.Count; j++)
                {
                    var modelStateObj = listlevel.ModelPrefab;
                    var obj = Instantiate(modelStateObj);
                    var itemStage = obj.GetComponent<LevelItem>();

                    itemStage.Initialized(null, null, null,
                        j + 1, j > currentLevel[i], j == listlevel.LevelItemData.Count - 1, j == 0,
                        historyLevel[j]
                    );
                    if (listlevel.LevelItemData[j].GemBonus != 0)
                    {
                        itemStage.SetActiveDown(true);
                    }

                    if (listlevel.LevelItemData[j].GoldBonus != 0)
                    {
                        itemStage.SetActiveTop(true);
                    }

                    view.AddLevelItem(obj.transform, i);
                }
            }
        }

        #endregion

        #region CallBack

        private void OnClickLevel(int stageIndex, int levelIndex)
        {
            // load game
            var param = PopupHelpers.PassParamPopup();
            var data = model.StageData.StageItemData[stageIndex].DataLevel.LevelItemData[levelIndex];

            param.SaveObject("GoldReward", data.LevelReward.Where(o => o.RewardType == Enums.RewardType.Coin));
            param.SaveObject("GemReward", data.LevelReward.Where(o => o.RewardType == Enums.RewardType.Gem));
            param.SaveObject("PlayerPosition", 0);
            param.SaveObject("TargetPosition", 0);
            param.SaveObject("MapSize", 0);
        }

        private void OnClickHome()
        {
            view.CloseAllPopup();
        }

        private void OnClickSetting()
        {
            var newParam = PopupHelpers.PassParamPopup();
            newParam.SaveObject("Title", "Error: Setting not implement");
            newParam.SaveObject("Detail", "Chức năng này chưa được hiện thực");
            PopupHelpers.Show(Constants.ErrorPopup);
        }

        private void OnClickInventory()
        {
            var newParam = PopupHelpers.PassParamPopup();
            newParam.SaveObject("Title", "Error: Inventory not implement");
            newParam.SaveObject("Detail", "Chức năng này chưa được hiện thực");
            PopupHelpers.Show(Constants.ErrorPopup);
        }

        private void OnClickShop()
        {
            var newParam = PopupHelpers.PassParamPopup();
            newParam.SaveObject("Title", "Error: Shop not implement");
            newParam.SaveObject("Detail", "Chức năng này chưa được hiện thực");
            PopupHelpers.Show(Constants.ErrorPopup);
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
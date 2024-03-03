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
        private int stageIndex = -1;
        private bool openPopup;

        private void Awake()
        {
            var gameServices = GameServices.Instance;
            playerService = gameServices.GetService<PlayerService>();
            currentLevel = playerService.CurrentLevel;
        }

        private void Start()
        {
            // main 
            LoadParam();
            InitMain();

            // popup
            view.InitializeLevel(model.StageData.StageItemData.Count, OnClickHome);
            view.InitializeStage(OnClickHome);

            // Create State
            InitStage();
            InitLevel();

            //
            if (openPopup)
            {
                OnClickStage(stageIndex);
            }
        }

        #region Initialized

        private void LoadParam()
        {
            var param = PopupHelpers.PassParamPopup();
            stageIndex = param.GetObject<int>(ParamType.StageIndex);
            openPopup = param.GetObject<bool>("OpenPopup");
        }

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

                    var j1 = j;
                    var i1 = i;
                    itemStage.Initialized(() => { OnClickLevel(i1, j1); }, null, null,
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
            param.SaveObject(ParamType.LevelData, data);
            param.SaveObject(ParamType.StageIndex, stageIndex);
            param.SaveObject(ParamType.LevelIndex, levelIndex);
            param.SaveObject(ParamType.PreviousLevel, levelIndex != playerService.CurrentLevel[stageIndex]);
            SceneManager.LoadScene(Constants.GamePlay);
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
            view.OpenLevel(model.StageData.StageItemData[index].Detail, playerService.UserCoin,
                playerService.UserDiamond, index);
        }

        #endregion
    }
}
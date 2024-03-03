using System.Collections.Generic;
using MainScene.Element;
using Services;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utilities;

namespace MainScene
{
    public class MainSceneController : MonoBehaviour
    {
        [Header("MVC")]
        [SerializeField] private MainSceneModel model;
        [SerializeField] private MainSceneView view;
        [Header("MainMenu Button")]
        [SerializeField] private Button playButton;
        [SerializeField] private Button shopButton;
        [SerializeField] private Button inventoryButton;
        [SerializeField] private Button userButton;
        [SerializeField] private Button settingButton;
        [Header("SYSTEM")]
        private PlayerService playerService;
        private APIService apiService;
        private AudioService audioService;

        private List<int> currentLevel;
        private int stageIndex = -1;
        private bool openPopup;

        private void Awake()
        {
            var gameServices = GameServices.Instance;
            playerService = gameServices.GetService<PlayerService>();
            apiService = gameServices.GetService<APIService>();
        }

        private void Start()
        {
            // main 
            InitMain();
            AssignButton();
            
        }

        #region Initialized

        private void AssignButton()
        {
            userButton.onClick.AddListener(OnClickUser);
            shopButton.onClick.AddListener(OnClickShop);
            settingButton.onClick.AddListener(OnClickSetting);
            inventoryButton.onClick.AddListener(OnClickInventory);
            playButton.onClick.AddListener(OnClickPlay);
        }

        private void InitMain()
        {
            view.SetDisplayUserCoin(playerService.UserCoin);
            view.SetDisplayUserName("Guest");
            view.SetDisplayUserEnergy(60, 60);
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
           //     itemStage.Initialized(item.Render, item.Detail, () => { OnClickStage(i1); });
              //  view.AddStageItem(obj.transform);
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

            //        view.AddLevelItem(obj.transform, i);
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
          //  param.SaveObject(ParamType.PreviousLevel, levelIndex != playerService.CurrentLevel[stageIndex]);
            SceneManager.LoadScene(Constants.GamePlay);
        }
        private void OnClickPlay()
        {
            PopupHelpers.ShowError("Chức năng này chưa được hiện thực");
        }
        
        private void OnClickSetting()
        {
            PopupHelpers.ShowError("Chức năng này chưa được hiện thực");
        }

        private void OnClickInventory()
        {
            PopupHelpers.ShowError("Chức năng này chưa được hiện thực");
        }

        private void OnClickShop()
        {
            PopupHelpers.ShowError("Chức năng này chưa được hiện thực");
        }

        private void OnClickUser()
        {
            if (string.IsNullOrEmpty(apiService.Jwt))
            {
                PopupHelpers.Show(Constants.LoginPopup);
            }
            else
            {
                PopupHelpers.ShowError("Chức năng này chưa được hiện thực");
            }
        }
        

        #endregion
    }
}
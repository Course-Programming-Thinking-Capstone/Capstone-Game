using System.Collections.Generic;
using GenericPopup.GameModeSelect;
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
        private ServerSideService serverSideService;
        private AudioService audioService;

        private List<int> currentLevel;
        private int stageIndex = -1;
        private bool openPopup;

        private void Awake()
        {
            var gameServices = GameServices.Instance;
            playerService = gameServices.GetService<PlayerService>();
            serverSideService = gameServices.GetService<ServerSideService>();
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

    
        #endregion

        #region CallBack

      
        private void OnClickPlay()
        {
            PopupHelpers.Show(Constants.GameModePopup);
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
            if (string.IsNullOrEmpty(serverSideService.Jwt))
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
using System.Collections.Generic;
using MainScene.MainPopup;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MainScene
{
    public class MainSceneView : MonoBehaviour
    {
        [Header("MainMenu")]
        [SerializeField] private TextMeshProUGUI userCoin;
        [SerializeField] private TextMeshProUGUI userDiamond;
        [SerializeField] private TextMeshProUGUI userName;
        [SerializeField] private TextMeshProUGUI userLevel;
        [SerializeField] private TextMeshProUGUI userProcess;
        [SerializeField] private Slider userProcessSlider;
        [Header("MainMenu Button")]
        [SerializeField] private Button playButton;
        [SerializeField] private Button shopButton;
        [SerializeField] private Button inventoryButton;
        [SerializeField] private Button settingButton;
        [Header("Layout")]
        [SerializeField] private StageSelect stagePopup;
        [SerializeField] private List<LevelSelect> levelPopup;

        [SerializeField] private Transform popupContainer;
        [SerializeField] private GameObject levelSelectPrefab;

        public void AddStageItem(Transform itemTf)
        {
            stagePopup.AddElement(itemTf);
            itemTf.localScale = Vector3.one;
        }

        public void AddLevelItem(Transform itemTf, int index)
        {
            levelPopup[index].AddElement(itemTf);
            itemTf.localScale = Vector3.one;
        }

        #region MAIN MENU

        public void SetDisplayUserCoin(int value)
        {
            userCoin.text = value.ToString();
        }

        public void SetDisplayUserDiamond(int value)
        {
            userDiamond.text = value.ToString();
        }

        public void SetDisplayUserName(string value)
        {
            userName.text = value;
        }

        public void SetDisplayUserLevel(int value)
        {
            userLevel.text = value.ToString();
        }

        public void SetDisplayUserProcess(string value, float fillValue)
        {
            userProcess.text = value;
            userProcessSlider.value = fillValue > 1 ? 1 : fillValue;
        }

        #endregion

        #region Init

        public void InitializedMain(
            UnityAction playCallBack,
            UnityAction shopCallBack,
            UnityAction inventCallBack,
            UnityAction settingCallBack)
        {
            playButton.onClick.AddListener(playCallBack);
            shopButton.onClick.AddListener(shopCallBack);
            inventoryButton.onClick.AddListener(inventCallBack);
            settingButton.onClick.AddListener(settingCallBack);
        }

        public void InitializeStage(UnityAction closeAll)
        {
            stagePopup.Initialized(closeAll);
        }

        public void InitializeLevel(int numberOfStage, UnityAction closeAll)
        {
            for (int i = 0; i < numberOfStage; i++)
            {
                var objTf = Instantiate(levelSelectPrefab, popupContainer).transform;
                var script = objTf.GetComponent<LevelSelect>();
                script.Initialized(closeAll);
                levelPopup.Add(script);
            }
        }

        #endregion

        #region OPEN/CLOSE POPUP

        public void CloseAllPopup()
        {
            stagePopup.gameObject.SetActive(false);
            foreach (var item in levelPopup)
            {
                item.gameObject.SetActive(false);
            }
        }

        public void OpenStage(int coin, int gem)
        {
            stagePopup.SetWallet(coin, gem);
            stagePopup.gameObject.SetActive(true);
        }

        public void OpenLevel(string nameStage, int coin, int gem, int stage)
        {
            levelPopup[stage].SetName(nameStage);
            levelPopup[stage].SetWallet(coin, gem);
            levelPopup[stage].gameObject.SetActive(true);
        }

        #endregion
    }
}
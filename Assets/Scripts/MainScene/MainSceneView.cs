using TMPro;
using UnityEngine;

namespace MainScene
{
    public class MainSceneView : MonoBehaviour
    {
        [Header("MainMenu")]
        [SerializeField] private TextMeshProUGUI userCoin;
        [SerializeField] private TextMeshProUGUI userEnergy;
        [SerializeField] private TextMeshProUGUI userName;

        #region MAIN MENU

        public void SetDisplayUserCoin(int value)
        {
            userCoin.text = value.ToString();
        }

        public void SetDisplayUserEnergy(int current, int max)
        {
            userEnergy.text = current + " / " + max;
        }

        public void SetDisplayUserName(string value)
        {
            userName.text = value;
        }

        #endregion

        //[SerializeField] private TextMeshProUGUI userDiamond;
        //[SerializeField] private TextMeshProUGUI userLevel;
        // [SerializeField] private TextMeshProUGUI userProcess;
        // [SerializeField] private Slider userProcessSlider;

        // [Header("Layout")]
        // [SerializeField] private StageSelect stagePopup;
        // [SerializeField] private List<LevelSelect> levelPopup;
        //
        // [SerializeField] private Transform popupContainer;
        // [SerializeField] private GameObject levelSelectPrefab;

        // public void AddStageItem(Transform itemTf)
        // {
        //     stagePopup.AddElement(itemTf);
        //     itemTf.localScale = Vector3.one;
        // }
        //
        // public void AddLevelItem(Transform itemTf, int index)
        // {
        //     levelPopup[index].AddElement(itemTf);
        //     itemTf.localScale = Vector3.one;
        // }

        #region Init

        // public void InitializeLevel(int numberOfStage, UnityAction closeAll)
        // {
        //     for (int i = 0; i < numberOfStage; i++)
        //     {
        //         var objTf = Instantiate(levelSelectPrefab, popupContainer).transform;
        //         var script = objTf.GetComponent<LevelSelect>();
        //         script.Initialized(closeAll);
        //         levelPopup.Add(script);
        //     }
        // }

        #endregion

        #region OPEN/CLOSE POPUP

        // public void CloseAllPopup()
        // {
        //     stagePopup.gameObject.SetActive(false);
        //     foreach (var item in levelPopup)
        //     {
        //         item.gameObject.SetActive(false);
        //     }
        // }

        // public void OpenStage(int coin, int gem)
        // {
        //     stagePopup.SetWallet(coin, gem);
        //     stagePopup.gameObject.SetActive(true);
        // }
        //
        // public void OpenLevel(string nameStage, int coin, int gem, int stage)
        // {
        //     levelPopup[stage].SetName(nameStage);
        //     levelPopup[stage].SetWallet(coin, gem);
        //     levelPopup[stage].gameObject.SetActive(true);
        // }

        #endregion
    }
}
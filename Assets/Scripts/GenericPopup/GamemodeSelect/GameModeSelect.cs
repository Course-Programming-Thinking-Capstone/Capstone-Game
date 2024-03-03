using GenericPopup.GameModeSelect;
using MainScene.Element;
using Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GenericPopup.SimplePopup
{
    public class GameModeSelect : PopupAdditive
    {
        [Header("View")]
        [SerializeField] private Button backButton;
        [SerializeField] private TextMeshProUGUI coinTxt;
        [SerializeField] private TextMeshProUGUI energyTxt;
        [SerializeField] private GameObject gameModeItem;
        [SerializeField] private Transform contentContainer;
        [SerializeField] private Image modeBg;

        private ServerSideService serverSideService;

        private void Start()
        {
            serverSideService = GameServices.Instance.GetService<ServerSideService>();
            backButton.onClick.AddListener(ClosePopup);
            coinTxt.text = serverSideService.Coin.ToString();
            energyTxt.text = "60 / 60";

            for (int i = 1; i < 6; i++)
            {
                var index = i;
                var item = CreateGameModeItem();
                item.Initialized(null, ((GameMode)index).ToString(), () => { OnClickStage((GameMode)index); });
            }
        }

        private StageItem CreateGameModeItem()
        {
            var obj = Instantiate(gameModeItem, contentContainer);
            obj.transform.localScale = Vector3.one;
            return obj.GetComponent<StageItem>();
        }

        private void OnClickStage(GameMode mode)
        {
            Debug.Log(mode);
        }
    }
}
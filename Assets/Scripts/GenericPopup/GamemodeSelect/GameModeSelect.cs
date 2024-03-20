using System;
using Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace GenericPopup.GameModeSelect
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

        private ClientService clientService;

        private void Awake()
        {
            clientService = GameServices.Instance.GetService<ClientService>();
            clientService.OnFailed = err => { PopupHelpers.ShowError(err); };
        }

        private async void Start()
        {
            backButton.onClick.AddListener(ClosePopup);
            coinTxt.text = clientService.coin.ToString();
            energyTxt.text = "60 / 60";

            var modeData = await clientService.GetGameMode();
            if (modeData != null && modeData.Count > 0)
            {
                foreach (var item in modeData)
                {
                    var index = item.idMode;
                    var objet = CreateGameModeItem();
                    objet.Initialized(null, item.typeName, () => { OnClickStage(index); });
                }
            }
            else
            {
                for (int i = 1; i < 6; i++)
                {
                    var index = i;
                    var objet = CreateGameModeItem();
                    objet.Initialized(null, ((GameMode)index).ToString(), () => { OnClickStage(index); });
                }
            }
        }

        private StageItem CreateGameModeItem()
        {
            var obj = Instantiate(gameModeItem, contentContainer);
            obj.transform.localScale = Vector3.one;
            return obj.GetComponent<StageItem>();
        }

        private void OnClickStage(int mode)
        {
            var param = PopupHelpers.PassParamPopup();
            param.SaveObject(ParamType.ModeGame, mode);
            PopupHelpers.Show(Constants.LevelPopup);
        }
    }
}
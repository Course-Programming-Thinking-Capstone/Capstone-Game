using System.Collections.Generic;
using System.Linq;
using Services;
using Services.Response;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace GenericPopup.GameModeSelect
{
    public class GameModeSelect : PopupAdditive
    {
        [SerializeField]
        private List<GameMode> modesIndex;

        [Header("View")]
        [SerializeField] private Button backButton;
        [SerializeField] private TextMeshProUGUI coinTxt;
        [SerializeField] private TextMeshProUGUI energyTxt;
        [SerializeField] private GameObject gameModeItem;
        [SerializeField] private Transform contentContainer;
        [SerializeField] private Image modeBg;

        private ClientService clientService;
        private PlayerService playerService;

        private void Awake()
        {
            audioService = GameServices.Instance.GetService<AudioService>();
            clientService = GameServices.Instance.GetService<ClientService>();
            playerService = GameServices.Instance.GetService<PlayerService>();
            clientService.OnFailed = err =>
            {
                ActiveSafePanel(false);
                PopupHelpers.ShowError(err, "ERROR");
            };
        }

        private async void Start()
        {
            backButton.onClick.AddListener(ClosePopup);
            coinTxt.text = clientService.Coin.ToString();
            energyTxt.text = "60 / 60";

            loading.SetActive(true);
            var modeData = await clientService.GetGameMode();
            var userProcess = await clientService.GetUserProcess();
            loading.SetActive(false);

            if (modeData == null || modeData.Count == 0)
            {
                audioService.PlaySound(GUISound.Fail);
                PopupHelpers.ShowError("Not found any level, please contact our support or try again later",
                    "Notification");
                return;
            }

            if (modesIndex.Count != modesIndex.Distinct().Count())
            {
                audioService.PlaySound(GUISound.Fail);
                PopupHelpers.ShowError("List index mode not valid");
                return;
            }

            GameModeResponse previousMode = null;
            foreach (var modeLoaded in modesIndex)
            {
                foreach (var item in modeData)
                {
                    if ((GameMode)item.idMode == modeLoaded)
                    {
                        var isLocked = true;
                        if (previousMode != null)
                        {
                            var previousLevel = playerService.GetCurrentLevel(previousMode.idMode);
                            if (previousLevel >= Constants.FreeLevel)
                            {
                                isLocked = false;
                            }

                            if (clientService.IsLogin)
                            {
                                var obj = userProcess.FirstOrDefault(o => o.mode == (GameMode)previousMode.idMode);
                                if (obj != null && obj.PlayedLevel.Count > 0)
                                {
                                    var max = obj.PlayedLevel.Max();
                                    if (max >= Constants.FreeLevel)
                                    {
                                        isLocked = false;
                                    }
                                }
                            }
                        }
                        else
                        {
                            isLocked = false;
                        }

                        previousMode = item;
                        var index = item.idMode;
                        var objet = CreateGameModeItem();
                        objet.Initialized(null, item.typeName, () => { OnClickStage(index); }, isLocked);
                    }
                }
            }
        }

        protected override void ClosePopup()
        {
            audioService.PlaySound(GUISound.Click);
            base.ClosePopup();
        }

        private StageItem CreateGameModeItem()
        {
            var obj = Instantiate(gameModeItem, contentContainer);
            obj.transform.localScale = Vector3.one;
            return obj.GetComponent<StageItem>();
        }

        private void OnClickStage(int mode)
        {
            audioService.PlaySound(GUISound.Play);
            var param = PopupHelpers.PassParamPopup();
            param.SaveObject(ParamType.ModeGame, mode);
            PopupHelpers.Show(Constants.LevelPopup);
        }
    }
}
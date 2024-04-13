using Services;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Utilities;

namespace GenericPopup.SimplePopup
{
    public class Pause : PopupAdditive
    {
        [SerializeField] private TextMeshProUGUI txt;
        private ClientService clientService;
        private UnityAction onReset;
        private int gameMode;
        private int levelIndex;

        private void Awake()
        {
            clientService = GameServices.Instance.GetService<ClientService>();
            clientService.OnFailed = err =>
            {
                ActiveSafePanel(false);
                PopupHelpers.ShowError(err, "ERROR");
            };
        }

        private void Start()
        {
            var parameter = PopupHelpers.PassParamPopup();
            gameMode = parameter.GetObject<int>(ParamType.ModeGame);
            levelIndex = parameter.GetObject<int>(ParamType.LevelIndex);

            txt.text = ((GameMode)gameMode) + " Mode: " + "Level " + (levelIndex+1);
        }

        public void OnClickSetting()
        {
            var param = PopupHelpers.PassParamPopup();
            param.SaveObject(ParamType.IsInGame, true);
            PopupHelpers.Show(Constants.SettingPopup);
        }

        public void OnClickContinue()
        {
            ClosePopup();
        }

        public void OnClickReset()
        {
            var param = PopupHelpers.PassParamPopup();
            param.SaveObject(ParamType.LevelIndex, levelIndex);
            switch ((GameMode)gameMode)
            {
                case GameMode.Basic:
                    SceneManager.LoadScene(Constants.BasicMode);
                    break;
                case GameMode.Sequence:
                    SceneManager.LoadScene(Constants.SequenceMode);
                    break;
                case GameMode.Loop:
                    SceneManager.LoadScene(Constants.LoopMode);
                    break;
                case GameMode.Function:
                    SceneManager.LoadScene(Constants.FuncMode);
                    break;
                case GameMode.Condition:
                    SceneManager.LoadScene(Constants.ConditionMode);
                    break;
            }
        }

        public void OnClickExit()
        {
            SceneManager.LoadScene(Constants.MainMenu);
        }
    }
}
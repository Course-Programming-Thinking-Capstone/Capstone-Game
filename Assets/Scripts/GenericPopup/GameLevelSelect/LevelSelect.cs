using System.Collections.Generic;
using System.Linq;
using Services;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utilities;

namespace GenericPopup.GameLevelSelect
{
    public class LevelSelect : PopupAdditive
    {
        [Header("Specific")]
        [SerializeField] private Button backButton;
        [SerializeField] private TextMeshProUGUI coinTxt;
        [SerializeField] private TextMeshProUGUI energyTxt;
        [SerializeField] private TextMeshProUGUI modeName;
        [SerializeField] private Transform contentContainer;
        [SerializeField] private GameObject levelItem;

        private ClientService clientService;
        private PlayerService playerService;

        private int gameMode;

        private void Awake()
        {
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
            var param = PopupHelpers.PassParamPopup();
            gameMode = param.GetObject<int>(ParamType.ModeGame);

            backButton.onClick.AddListener(ClosePopup);
            coinTxt.text = clientService.Coin.ToString();
            energyTxt.text = "60 / 60";
            modeName.text = gameMode.ToString();
            var currentLocalPlayed = playerService.GetCurrentLevel(gameMode);
            var userPlayedLevel = new List<int>();
            var baseUnlockLevel = Constants.FreeLevel;
            var allLevel = 10;

            if (clientService.GameModes.TryGetValue(gameMode, out var mode))
            {
                allLevel = mode.totalLevel;
                modeName.text = mode.typeName;

                var currentLeveList = await clientService.GetUserProcess();
                foreach (var process in currentLeveList)
                {
                    if ((int)process.mode == gameMode)
                    {
                        userPlayedLevel = process.PlayedLevel;
                        break;
                    }
                }
            }

            // Create Level
            for (int i = 0; i < allLevel; i++)
            {
                var item = CreateLevelItem();
                item.gameObject.SetActive(true);
                var index = i;
                var isPlayed = false;
                var isLocked = true;

                if (clientService.UserId != -1) // already login
                {
                    if (i <= userPlayedLevel.Max() + 1)
                    {
                        isLocked = false;
                    }

                    if (userPlayedLevel.Contains(i)) // Already play this level
                    {
                        isPlayed = true;
                    }
                }
                else // using local
                {
                    // Local handle level
                    if (i < baseUnlockLevel)
                    {
                        if (i <= currentLocalPlayed) // unlock all first 3
                        {
                            isLocked = false;
                        }

                        if (i < currentLocalPlayed)
                        {
                            isPlayed = true;
                        }
                    }
                }

                item.Initialized(
                    i,
                    isPlayed,
                    isLocked, () => { OnClickLevel(index); }, i == allLevel - 1
                );
            }
        }

        private LevelItem CreateLevelItem()
        {
            var obj = Instantiate(levelItem, contentContainer);
            obj.transform.localScale = Vector3.one;
            return obj.GetComponent<LevelItem>();
        }

        private void OnClickLevel(int index)
        {
            var param = PopupHelpers.PassParamPopup();
            param.SaveObject(ParamType.LevelIndex, index);
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
    }
}
using System;
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

        private int currentLevel;
        private int allLevel;

        private int gameMode;

        private void Awake()
        {
            clientService = GameServices.Instance.GetService<ClientService>();
            var param = PopupHelpers.PassParamPopup();
            gameMode = param.GetObject<int>(ParamType.ModeGame);
            Destroy(param.gameObject);
        }

        private void Start()
        {
            backButton.onClick.AddListener(ClosePopup);
            coinTxt.text = clientService.coin.ToString();
            energyTxt.text = "60 / 60";
            modeName.text = gameMode.ToString();

            // Test
            if (clientService.GameModes.TryGetValue(gameMode, out var mode))
            {
                currentLevel = mode.totalLevel;
                allLevel = mode.totalLevel;
                modeName.text = mode.typeName;
            }
            else
            {
                currentLevel = 5;
                allLevel = 10;
            }

            // Create Level
            for (int i = 0; i < allLevel; i++)
            {
                var item = CreateLevelItem();
                var index = i;
                item.Initialized(
                    i, i < currentLevel,
                    i > currentLevel, () => { OnClickLevel(index); }, i == allLevel - 1
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
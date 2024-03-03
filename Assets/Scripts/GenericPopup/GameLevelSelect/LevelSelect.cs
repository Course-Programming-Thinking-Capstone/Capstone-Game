using Services;
using TMPro;
using UnityEngine;
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

        private ServerSideService serverSideService;

        private int currentLevel;
        private int allLevel;

        private GameMode gameMode;

        private void Awake()
        {
            var param = PopupHelpers.PassParamPopup();
            gameMode = param.GetObject<GameMode>(ParamType.ModeGame);
        }

        private void Start()
        {
            serverSideService = GameServices.Instance.GetService<ServerSideService>();
            backButton.onClick.AddListener(ClosePopup);
            coinTxt.text = serverSideService.Coin.ToString();
            energyTxt.text = "60 / 60";
            modeName.text = gameMode.ToString();

            // Test
            currentLevel = 10;
            allLevel = 50;
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
            Debug.Log(gameMode);
            Debug.Log(index);
        }
    }
}
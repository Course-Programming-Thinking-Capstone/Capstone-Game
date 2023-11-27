using TMPro;
using UnityEngine;
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
    }
}
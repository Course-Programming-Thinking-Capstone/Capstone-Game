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
    }
}
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace GenericPopup
{
    public class ErrorPopup : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI header;
        [SerializeField] private TextMeshProUGUI detail;
        [SerializeField] private Button okButton;
        [SerializeField] private Button closeButton;

        private void Start()
        {
            var parameter = PopupHelpers.PassParamPopup();

            if (parameter == null)
            {
                PopupHelpers.Close();
            }

            var callBack = parameter.GetAction(ActionType.YesOption);
            if (callBack != null)
            {
                okButton.onClick.AddListener(callBack);
                closeButton.onClick.AddListener(callBack);
            }

            okButton.onClick.AddListener(PopupHelpers.Close);
            closeButton.onClick.AddListener(PopupHelpers.Close);

            header.text = parameter.GetObject<string>("Title");
            detail.text = parameter.GetObject<string>("Detail");
        }
    }
}
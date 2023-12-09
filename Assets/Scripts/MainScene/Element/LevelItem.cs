using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MainScene.Element
{
    public class LevelItem : MonoBehaviour
    {
        [SerializeField] private GameObject[] path;

        [SerializeField] private Image imageUp;
        [SerializeField] private Image imageCenter;
        [SerializeField] private Image imageDown;

        [SerializeField] private Button btnUp;
        [SerializeField] private Button btnCenter;
        [SerializeField] private Button btnDown;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callBackCenter"></param>
        /// <param name="callBackUp"></param>
        /// <param name="callBackDown"></param>
        /// <param name="pathActive"> Left Right</param>
        public void Initialized(
            Sprite spriteCenter
            , UnityAction callBackCenter, UnityAction callBackUp, UnityAction callBackDown
            , bool[] pathActive
        )
        {
            btnUp.onClick.AddListener(callBackUp);
            btnCenter.onClick.AddListener(callBackCenter);
            btnDown.onClick.AddListener(callBackDown);

            imageCenter.sprite = spriteCenter;
            path[0].SetActive(pathActive[0]);
            btnUp.gameObject.SetActive(pathActive[0]);

            path[1].SetActive(pathActive[1]);
            btnDown.gameObject.SetActive(pathActive[1]);
            
            path[2].SetActive(pathActive[2]);
            path[3].SetActive(pathActive[3]);

            imageUp.SetNativeSize();
            imageCenter.SetNativeSize();
            imageDown.SetNativeSize();
        }
    }
}
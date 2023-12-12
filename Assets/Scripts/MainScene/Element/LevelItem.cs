using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MainScene.Element
{
    public class LevelItem : MonoBehaviour
    {
        [SerializeField] private GameObject[] path;
        [SerializeField] private GameObject[] stars;

        [SerializeField] private GameObject unlockUp;
        [SerializeField] private GameObject unlockCenter;
        [SerializeField] private GameObject unlockDown;

        [SerializeField] private GameObject lockedUp;
        [SerializeField] private GameObject lockedCenter;
        [SerializeField] private GameObject lockedDown;

        [SerializeField] private List<TextMeshProUGUI> textLevel;

        [SerializeField] private Button btnUp;
        [SerializeField] private Button btnCenter;
        [SerializeField] private Button btnDown;

        public void SetCenter(bool isLocked, int level)
        {
            lockedCenter.SetActive(isLocked);
            unlockCenter.SetActive(!isLocked);
            ChangeLevelText(level.ToString());
        }

        public void SetActiveTop(bool isLocked)
        {
            lockedUp.SetActive(isLocked);
            unlockUp.SetActive(!isLocked);
            path[0].SetActive(true);
        }

        public void SetActiveDown(bool isLocked)
        {
            lockedDown.SetActive(isLocked);
            unlockDown.SetActive(!isLocked);
            path[1].SetActive(true);
        }

        public void ChangeLevelText(string txt)
        {
            foreach (var item in textLevel)
            {
                item.text = txt;
            }
        }

        public void Initialized(
            UnityAction callBackCenter, UnityAction callBackUp, UnityAction callBackDown,
            int level,
            bool isCenterLocker, bool isRight, bool isLeft, int star = 0
        )
        {
            btnUp.onClick.AddListener(callBackUp);
            btnCenter.onClick.AddListener(callBackCenter);
            btnDown.onClick.AddListener(callBackDown);

            path[2].SetActive(!isLeft);
            path[3].SetActive(!isRight);

            SetCenter(isCenterLocker, level);

            if (star != 0)
            {
                for (int i = 0; i < star; i++)
                {
                    stars[i].SetActive(true);
                }
            }
        }
    }
}
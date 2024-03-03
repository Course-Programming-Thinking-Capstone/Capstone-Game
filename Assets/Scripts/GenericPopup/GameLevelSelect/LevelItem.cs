using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GenericPopup.GameLevelSelect
{
    public class LevelItem : MonoBehaviour
    {
        [SerializeField] private GameObject lockItem;
        [SerializeField] private GameObject unlockItem;
        [SerializeField] private GameObject playedItem;
        [SerializeField] private GameObject pathLeft;
        [SerializeField] private GameObject pathRight;

        [SerializeField] private List<TextMeshProUGUI> textLevel;

        [SerializeField] private List<Button> btnCenter;

        public void Initialized(int level, bool isPlayed, bool isLocked, UnityAction callBack,
            bool isLast = false
        )
        {
            foreach (var btn in btnCenter)
            {
                btn.onClick.AddListener(callBack);
            }

            // text display
            foreach (var txt in textLevel)
            {
                txt.text = (level + 1).ToString();
            }

            // Path line
            if (level == 0)
            {
                pathLeft.SetActive(false);
            }

            pathRight.SetActive(!isLast);

            // Level item
            lockItem.SetActive(false);
            unlockItem.SetActive(false);
            playedItem.SetActive(false);
            if (isLocked)
            {
                lockItem.SetActive(true);
            }
            else
            {
                unlockItem.SetActive(!isPlayed);
                playedItem.SetActive(isPlayed);
            }
        }
    }
}
using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace GameScene
{
    public class PadSelectController : MonoBehaviour
    {
        [Header("For animation")]
        [SerializeField] private Button controlButton;
        [SerializeField] private Transform arrowDir;
        [SerializeField] private Transform selectorPad;
        [SerializeField] private Transform selectedPad;
        [SerializeField] private Transform baseXPositionA;
        [SerializeField] private Transform baseXPositionB;
        private bool isClose;

        private void Awake()
        {
            controlButton.onClick.AddListener(OnClickOpenClose);
        }

        private void OnClickOpenClose()
        {
            controlButton.interactable = false;
            if (isClose)
            {
                isClose = false;
                // Open
                OpenAnimation();
            }
            else
            {
                isClose = true;
                // Close
                CloseAnimation();
            }
        }

        private void OpenAnimation()
        {
            arrowDir.DORotate(new Vector3(0, 0, 0), 0.75f).OnComplete(() => { controlButton.interactable = true; });
            selectorPad.DOMoveX(baseXPositionA.position.x, 0.5f);
            selectedPad.DOMoveX(baseXPositionB.position.x, 0.5f);
        }

        private void CloseAnimation()
        {
            arrowDir.DORotate(new Vector3(0, 180, 0), 0.75f).OnComplete(() => { controlButton.interactable = true; });
            selectorPad.DOMoveX(0, 0.5f);
            selectedPad.DOMoveX(0, 0.5f);
        }
    }
}
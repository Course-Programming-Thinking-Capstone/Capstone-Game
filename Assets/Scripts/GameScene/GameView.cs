using System.Collections.Generic;
using UnityEngine;

namespace GameScene
{
    public class GameView  : MonoBehaviour
    {
        [SerializeField] private Transform selectorContainer;
        [SerializeField] private Transform selectedContainer;

        public void SetParentSelector(Transform child)
        {
            child.SetParent(selectorContainer);
            child.localScale = Vector3.one;
        }
        
        public void SetParentSelected(Transform child)
        {
            child.SetParent(selectedContainer);
            child.localScale = Vector3.one;
        }
        
        public void SetPositionSelected(RectTransform item, int index)
        {
            var yPosition = -item!.sizeDelta.y * (index - 0.5f);
            item!.anchoredPosition = new Vector3(0f, yPosition, 0f);
        }
        
        public void ReSortItemsSelected(List<RectTransform> items)
        {
            for (int i = 0; i < items.Count; i++)
            {
                SetPositionSelected(items[i], i +1);
            }
        }

        public void MakeEmptySpace(List<RectTransform> items, int indexToMakeSpace)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (indexToMakeSpace == i)
                {
                    continue;
                }
                SetPositionSelected(items[i], i + 1);
            }
        }

    
        
    }
}
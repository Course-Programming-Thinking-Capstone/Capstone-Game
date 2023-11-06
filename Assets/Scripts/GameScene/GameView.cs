using System.Collections.Generic;
using UnityEngine;

namespace GameScene
{
    public class GameView  : MonoBehaviour
    {
        [SerializeField] private Transform selectorContainer;
        [SerializeField] private Transform selectedContainer;
        [SerializeField] private RectTransform playZone;

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

        public void InitPlayerPosition(RectTransform playerTransform, Vector2 map, Vector2 playerPos)
        {
            playerTransform.SetParent(playZone);
            // Get the rectangular bounding box of your UI element
            var rect = playZone.rect;
            var anchoredPosition = playZone.anchoredPosition;
            
            var cellXSize = rect.width / map.x;
            var cellYSize = rect.height / map.x;
            
            var lefBottomSize = new Vector2(anchoredPosition.x - rect.width / 2f, anchoredPosition.y - rect.height / 2f);

            var playerPosToSet = lefBottomSize ;
            playerPosToSet.x += cellXSize * (playerPos.x - 0.5f);
            playerPosToSet.y += cellYSize * (playerPos.y - 0.5f);
            playerTransform.anchoredPosition = playerPosToSet;
        }
        
        
    }
}
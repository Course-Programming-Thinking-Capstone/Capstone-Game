using System.Collections.Generic;
using UnityEngine;

namespace GameScene.GameCondition
{
    public class ConditionView : GameView
    {
        #region Canvas

        /// <summary>
        /// Add and object to selected list with index
        /// </summary>
        private void SetPositionSelected(RectTransform item, float yPosition)
        {
            item.anchoredPosition = new Vector3(0f, yPosition, 0f);
        }

   
        public void MakeEmptySpace(List<RectTransform> items, int indexToMakeSpace, float sizeSpace)
        {
            var yPosition = 0f;
            for (int i = 0; i < items.Count; i++)
            {
                yPosition += -items[i].sizeDelta.y / 2;
                if (indexToMakeSpace == i)
                {
                    yPosition += -sizeSpace;
                }

                SetPositionSelected(items[i], yPosition);
                yPosition += -items[i].sizeDelta.y / 2;
            }
        }
  
        #endregion
    }
}

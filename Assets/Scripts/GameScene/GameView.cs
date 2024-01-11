using System.Collections.Generic;
using UnityEngine;

namespace GameScene
{
    public class GameView : MonoBehaviour
    {
        [Header("2D references")]
        [SerializeField] protected GameObject savePanel;
        [SerializeField] protected Transform startGroundPosition;
        [SerializeField] protected Transform container;
        [SerializeField] protected Transform blockContainer;
        
        [Header("Cache")]
        protected readonly List<Vector2> positions = new();
        protected Vector2 boardSize;

        public void ActiveSavePanel(bool isActive = true)
        {
            savePanel.SetActive(isActive);
        }

        #region Initialize

        public void InitGroundBoard(List<Transform> groundItems, Vector2 board, float offSet)
        {
            boardSize = board;
            var sizeY = (int)board.y;
            var sizeX = (int)board.x;
            for (int i = 0; i < sizeY; i++) // vertical
            {
                for (int j = 0; j < sizeX; j++) // horizontal
                {
                    var positionNew = startGroundPosition.position;
                    positionNew.x += offSet * j;
                    positionNew.y += offSet * i;
                    groundItems[i * sizeX + j].position = positionNew;
                    positions.Add(positionNew);
                    groundItems[i * sizeX + j].SetParent(blockContainer);
                }
            }
        }
        
        /// <summary>
        /// Place any object to board position
        /// </summary>
        /// <param name="objectToSet"></param>
        /// <param name="playerPos"></param>
        public void PlaceObjectToBoard(Transform objectToSet, Vector2 playerPos)
        {
            objectToSet.SetParent(container);
            objectToSet.position = GetPositionFromBoard(playerPos);
        }
        
        /// <summary>
        /// Get 2d position from board index
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public Vector2 GetPositionFromBoard(Vector2 position)
        {
            var index = (int)((position.y - 1) * boardSize.x + (position.x - 1));
       
            return positions[index];
        }

        #endregion
    }
}
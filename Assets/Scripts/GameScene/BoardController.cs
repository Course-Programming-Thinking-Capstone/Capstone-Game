using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameScene
{
    public class BoardController : MonoBehaviour
    {
        [SerializeField] private Transform startGroundPosition;
        [SerializeField] private Transform groundContainer;
        private Vector2 boardSize;
        private List<Transform> cacheBoardItem;
        //       private readonly float offSet = 0.2f;

        private void Awake()
        {
            cacheBoardItem = new List<Transform>();
        }

        public void CreateBoard(Vector2 baseBoardSize, GameObject modelBoard)
        {
            var renderer = modelBoard.GetComponentInChildren<SpriteRenderer>();
            var cellSize = renderer.size * renderer.transform.localScale;
            for (int i = 0; i < baseBoardSize.x * baseBoardSize.y; i++)
            {
                var obj = Instantiate(modelBoard, groundContainer);
                obj.transform.localScale = Vector3.one;
                cacheBoardItem.Add(obj.transform);
            }

            var sizeY = (int)baseBoardSize.y;
            var sizeX = (int)baseBoardSize.x;
            for (int i = 0; i < sizeY; i++) // vertical
            {
                for (int j = 0; j < sizeX; j++) // horizontal
                {
                    var positionNew = startGroundPosition.position;
                    positionNew.x += cellSize.x * j;
                    positionNew.y += cellSize.y * i;
                    cacheBoardItem[i * sizeX + j].position = positionNew;
                }
            }
        }
    }
}
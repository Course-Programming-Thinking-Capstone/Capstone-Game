using System.Collections.Generic;
using Services;
using UnityEngine;

namespace GameScene
{
    public class BoardController : MonoBehaviour
    {
        [SerializeField] private float offsetBoard;
        [SerializeField] private Transform startGroundPosition;
        [SerializeField] private Transform groundContainer;
        [SerializeField] private Transform objectPlacedContainer;
        private Vector2 boardSize;
        private List<Transform> cacheBoardItem;
        private List<Animator> cacheBoardAnimator;
       

        private void Awake()
        {
            cacheBoardItem = new List<Transform>();
            cacheBoardAnimator = new List<Animator>();
        }

        public void CreateBoard(Vector2 baseBoardSize, GameObject modelBoard)
        {
            boardSize = baseBoardSize;
            var spriteRenderer = modelBoard.GetComponentInChildren<SpriteRenderer>();
            var cellSize = spriteRenderer.size * spriteRenderer.transform.localScale;
            for (int i = 0; i < baseBoardSize.x * baseBoardSize.y; i++)
            {
                var obj = Instantiate(modelBoard, groundContainer);
                obj.transform.localScale = Vector3.one;
                cacheBoardItem.Add(obj.transform);
                cacheBoardAnimator.Add(obj.GetComponent<Animator>());
            }

            var sizeY = (int)baseBoardSize.y;
            var sizeX = (int)baseBoardSize.x;
            for (int i = 0; i < sizeY; i++) // vertical
            {
                for (int j = 0; j < sizeX; j++) // horizontal
                {
                    var positionNew = startGroundPosition.position;
                    positionNew.x += (offsetBoard + cellSize.x) * j;
                    positionNew.y += (offsetBoard + cellSize.y) * i;
                    cacheBoardItem[i * sizeX + j].position = positionNew;
                }
            }
        }

        public void ActiveSpecificBoard(List<Vector2> neededBoard)
        {
            foreach (var board in cacheBoardItem)
            {
                board.gameObject.SetActive(false);
            }

            foreach (var position in neededBoard)
            {
                cacheBoardItem[(int)((position.y - 1) * boardSize.x + (position.x - 1))].gameObject.SetActive(true);
            }
        }

        public List<T> ActiveSpecificBoard<T>(List<Vector2> neededBoard)
        {
            var result = new List<T>();
            foreach (var board in cacheBoardItem)
            {
                board.gameObject.SetActive(false);
            }

            foreach (var position in neededBoard)
            {
                var obj = cacheBoardItem[(int)((position.y - 1) * boardSize.x + (position.x - 1))].gameObject;
                obj.SetActive(true);
                result.Add(obj.GetComponent<T>());
            }

            return result;
        }

        public T GetPartAtPosition<T>(Vector2 targetPosition)
        {
            var obj = cacheBoardItem[(int)((targetPosition.y - 1) * boardSize.x + (targetPosition.x - 1))].gameObject;

            return obj.GetComponent<T>();
        }

        /// <summary>
        /// Place any object into to board
        /// </summary>
        /// <param name="objectToSet"></param>
        /// <param name="playerPos"></param>
        public void PlaceObjectToBoard(Transform objectToSet, Vector2 playerPos)
        {
            objectToSet.SetParent(objectPlacedContainer);
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
            return cacheBoardItem[index].position;
        }
   
        public void SetGroundAnimation(Vector2 position, bool isOn)
        {
            var index = (int)((position.y - 1) * boardSize.x + (position.x - 1));
            var animator = cacheBoardAnimator[index];
            animator.SetBool("IsOn", isOn);
        }
    }
}
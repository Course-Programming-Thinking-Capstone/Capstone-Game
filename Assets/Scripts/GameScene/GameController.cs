using System.Collections.Generic;
using System.Linq;
using GameScene.Component;
using JetBrains.Annotations;
using Services;
using UnityEngine;

namespace GameScene
{
    public class GameController : MonoBehaviour
    {
        [Header("Reference model")] [SerializeField]
        private GameView view;

        [SerializeField] private GameModel model;
        [SerializeField] private RectTransform deleteZone;

        [Header("Testing only")] [SerializeField]
        private List<SelectType> generateList;

        // SYSTEM
        private readonly List<Selector> storeSelector = new();
        private readonly List<Selector> storeSelected = new();
        [CanBeNull] private Selector selectedObject;
        private bool isDelete;

        #region INITIALIZE

        private void Awake()
        {
            // Load services
            if (GameObject.FindGameObjectWithTag(Constants.ServicesTag) != null)
            {
                var services = GameObject.FindGameObjectWithTag(Constants.ServicesTag).GetComponent<GameServices>();
            }
            else
            {
                //  SceneManager.LoadScene(Constants.EntryScene);
            }
        }

        private void Start()
        {
            InitScene();
        }

        private void InitScene()
        {
            // Generate objects selector
            foreach (var o in generateList)
            {
                var obj = Instantiate(model.GetSelector(o));
                view.SetParentSelector(obj.transform);
                storeSelector.Add(obj.GetComponent<Arrow>());
            }

            // Assign callback for selector
            foreach (var arrow in storeSelector)
            {
                arrow.Init(OnClickedSelector);
            }
        }

        #endregion

        private void Update()
        {
            if (selectedObject)
            {
                if (Input.GetMouseButtonUp(0))
                {
                    HandleMouseUp();
                }
                else
                {
                    MoveSelected();
                }
            }
        }

        private void MoveSelected()
        {
            Vector3 mousePos = Input.mousePosition;
            selectedObject!.RectTransform.position = mousePos;
            // handle if inside delete zone
            if (IsPointInRT(mousePos, deleteZone))
            {
                isDelete = true;
            }
            else
            {
                isDelete = false;
            }
        }

        bool IsPointInRT(Vector2 point, RectTransform rt)
        {
            // Get the rectangular bounding box of your UI element
            var rect = rt.rect;
            var anchoredPosition = rt.position;
            // Get the left, right, top, and bottom boundaries of the rect
            float leftSide = anchoredPosition.x - rect.width / 2f;
            float rightSide = anchoredPosition.x + rect.width / 2f;
            float topSide = anchoredPosition.y + rect.height / 2f;
            float bottomSide = anchoredPosition.y - rect.height / 2f;

            // Check to see if the point is in the calculated bounds
            if (point.x >= leftSide &&
                point.x <= rightSide &&
                point.y >= bottomSide &&
                point.y <= topSide)
            {
                return true;
            }

            return false;
        }

        private void HandleMouseUp()
        {
            if (isDelete) // in delete zone
            {
                storeSelected.Remove((Arrow)selectedObject);
                SimplePool.Despawn(selectedObject!.gameObject);
                view.ReSortItemsSelected(storeSelected.Select(o => o.RectTransform).ToList());
                selectedObject = null;
                isDelete = false;
            }
            else // Valid pos
            {
                if (!storeSelected.Contains(selectedObject))
                {
                    storeSelected.Add(selectedObject);
                }

                view.ReSortItemsSelected(storeSelected.Select(o => o.RectTransform).ToList());
                selectedObject = null;
            }
        }

        #region CALL BACK

        // Event clicked selector
        private void OnClickedSelector(Selector selectedObj)
        {
            // Generate new selected
            var obj = SimplePool.Spawn(model.GetSelected(selectedObj.SelectType));
            view.SetParentSelected(obj.transform);
            // Generate init selected
            var arrow = obj.GetComponent<Arrow>();
            arrow.Init(OnClickedSelected);
            // assign to control
            selectedObject = arrow;
        }

        private void OnClickedSelected(Selector selectedObj)
        {
            selectedObject = selectedObj;
        }

        #endregion
    }
}
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
        [SerializeField] private Canvas canvas;

        [Header("Testing only")] [SerializeField]
        private List<SelectType> generateList;

        // SYSTEM
        private List<Arrow> storeSelector = new();
        private List<Arrow> storeSelected = new();
        [CanBeNull] private Selector  selectedObject;
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
        }

        private void HandleMouseUp()
        {
            if (isDelete)
            {
                storeSelected.Remove((Arrow)selectedObject);
                SimplePool.Despawn(selectedObject!.gameObject);
                view.ReSortItemsSelected(storeSelected.Select(o => o.RectTransform).ToList());
                selectedObject = null;
                isDelete = false;
            }
            else
            {
                view.SetPositionSelected(selectedObject!.RectTransform, storeSelected.Count);
                selectedObject = null;
            }
            

        }

        #region CALL BACK

        // Event clicked selector
        private void OnClickedSelector(Selector selectedObject)
        {
            // Generate new selected
            var obj = SimplePool.Spawn(model.GetSelected(selectedObject.SelectType));
            view.SetParentSelected(obj.transform);
            // Generate init selected
            var arrow = obj.GetComponent<Arrow>();
            arrow.Init(OnClickedSelected);
            storeSelected.Add(arrow);
            
            // assign to control
            this.selectedObject = arrow;


        }
        private void OnClickedSelected(Selector selectedObject)
        {
            this.selectedObject = selectedObject;
            isDelete = true;
        }

        #endregion
     
    }
}
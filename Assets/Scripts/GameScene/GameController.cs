using System;
using System.Collections.Generic;
using GameScene.Component;
using JetBrains.Annotations;
using Services;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameScene
{
    public class GameController : MonoBehaviour
    {
        [Header("Reference model")] 
        [SerializeField] private GameView view;
        [SerializeField] private GameModel model;
        [SerializeField] private Canvas  canvas;

        [Header("Testing only")] 
        [SerializeField] private List<SelectType> generateList;

        // SYSTEM
        private List<Arrow> storeSelector = new();
        private List<Arrow> storeSelected = new();
        [CanBeNull] private RectTransform selectedTransform;
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

        private void Update()
        {
            if (Input.GetMouseButtonUp(0))
            {
       
                MoveSelectedToPosition();
            }
            
            if (selectedTransform)
            {
                MoveSelected();
             
            }

         
        }

        private void MoveSelected()
        {
            // Move with mouse
            Vector3 mousePos = Input.mousePosition;
            selectedTransform!.position = mousePos;
        }
        private void MoveSelectedToPosition()
        {
            selectedTransform = null;
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

        // Event clicked selector
        private void OnClickedSelector(SelectType selectType)
        {
            var obj = Instantiate(model.GetSelected(selectType));
            view.SetParentSelected(obj.transform);
            selectedTransform = obj.GetComponent<RectTransform>();
        }
    }
}
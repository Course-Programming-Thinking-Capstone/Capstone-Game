using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameScene
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private GameView view;
        [SerializeField] private GameModel model;

        // TESTING
        [SerializeField] private List<SelectType> generateList;
        
        private void Awake()
        {
            
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
                var obj =  Instantiate(model.GetSelector(o));
                view.SetParentSelector(obj.transform);
            }
        }
    }
}
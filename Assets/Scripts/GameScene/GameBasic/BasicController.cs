using System;
using System.Collections.Generic;
using GameScene.Component;
using GameScene.Component.GameBasic;
using GameScene.GameSequence;
using UnityEngine;

namespace GameScene.GameBasic
{
    public class BasicGameController : GameController
    {
        [Header("Reference model")]
        [SerializeField] private BasicView view;
        [SerializeField] private BasicModel model;
        // System
        private List<Transform> listBoard = new List<Transform>();
        // Demo
        private List<SelectType> original = new List<SelectType>()
        {
            SelectType.RoadHorizontal,
            SelectType.RoadHorizontal,
            SelectType.RoadVertical,
            SelectType.RoadTurn1,
            SelectType.RoadTurn2,
            SelectType.RoadTurn3,
            SelectType.RoadTurn4,
        };

        [SerializeField] Vector2 boardSize = Vector2.one;

        private void Start()
        {
            InitScene();
        }

        private void InitScene()
        {
            // Selector
            foreach (var item in original)
            {
                var newObj = Instantiate(model.RoadToSelect);
                view.AddRoadToContainer(newObj.transform);
                var scriptControl = newObj.GetComponent<Road>();
                scriptControl.Init(OnClickRoad);
                scriptControl.ChangeRender(model.GetSprite(item));
            }

            // Ground
            for (int i = 0; i < boardSize.x * boardSize.y; i++)
            {
                listBoard.Add(Instantiate(model.RoadGroundPrefab).transform);
            }

            view.InitGroundBoard(listBoard, boardSize, model.GetBlockOffset());
        }

        private void OnClickRoad(Selector road)
        {
        }
    }
}
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
        private Vector2 boardSize = new Vector2(8,6);
        // Demo, parameter need
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

        [SerializeField][Tooltip("Max 8x6")] private List<Vector2> roadPartPositions;

        private void Start()
        {
            GenerateGround();
            GenerateSelector();
        }

        private void GenerateSelector()
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
        }
        private void GenerateGround()
        {
            // Ground
            view.InitGroundBoardFakePosition(boardSize, model.GetBlockOffset());
            foreach (var positionRoad in roadPartPositions)
            {
                var newRoad = Instantiate(model.RoadGroundPrefab);
                listBoard.Add(newRoad.transform);
                view.PlaceGround(newRoad.transform, positionRoad);
            }
        }

        private void OnClickRoad(Selector road)
        {
        }
    }
}
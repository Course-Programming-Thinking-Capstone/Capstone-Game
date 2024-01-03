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

        private void Start()
        {
            InitScene();
        }

        private void InitScene()
        {
            foreach (var item in original)
            {
                var newObj = Instantiate(model.RoadToSelect);
                view.AddRoadToContainer(newObj.transform);
                var scriptControl = newObj.GetComponent<Road>();
                scriptControl.Init(OnClickRoad);
                scriptControl.ChangeImage.ChangeRender(model.GetSprite(item));
            }
        }

        private void OnClickRoad(Selector road)
        {
        }
    }
}
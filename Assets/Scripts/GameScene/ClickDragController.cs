using System.Collections.Generic;
using GameScene.Component;
using GameScene.Component.SelectControl;
using JetBrains.Annotations;
using UnityEngine;

namespace GameScene
{
    public class ClickDragController : GameController
    {
        [Header("Click and drag")]
        // FOR CONTROL SELECTOR
        protected readonly List<InteractionItem> storeSelector = new();
        protected readonly List<InteractionItem> storeSelected = new();
        protected readonly List<Vector2> storedPosition = new();
        protected bool isDelete;
        protected const float OffSet = 0.2f;
        [CanBeNull] protected InteractionItem selectedObject;

        // System
        protected Candy candy;
        protected Vector2 currentPlayerPosition;
        protected readonly Dictionary<Vector2, bool> targetChecker = new();
        protected readonly Dictionary<Vector2, Transform> targetReferences = new();

        [Header("Demo param")]
        [SerializeField]
        protected List<Vector2> boardMap;
        
        
    }
}
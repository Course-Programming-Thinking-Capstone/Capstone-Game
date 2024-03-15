using System.Collections.Generic;
using GameScene.Component;
using GameScene.Component.SelectControl;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace GameScene
{
    public class ClickDragController : GameController
    {
        [Header("Click and drag reference object game")]
        [SerializeField] protected Button playButton;
        [Header("Test Param")]
        [SerializeField] protected List<SelectType> generateList;
        [SerializeField] protected List<Vector2> boardMap;
        // System
        protected readonly Dictionary<Vector2, Transform> targetReferences = new();
        protected readonly Dictionary<Vector2, bool> targetChecker = new();
        protected Vector2 currentPlayerPosition;
        protected Target target;

        // Comment form here if finish
        [Header("Remove soon")]
        [SerializeField] protected RectTransform deleteZone;
        [SerializeField] protected RectTransform selectedZone;
        // FOR CONTROL SELECTOR
        protected readonly List<InteractionItem> storeSelected = new();
        protected readonly List<Vector2> storedPosition = new();
        protected bool isDelete;
        protected const float OffSet = 0.2f;
        [CanBeNull] protected InteractionItem selectedObject;
    }
}
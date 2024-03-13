using System;
using System.Collections.Generic;
using DG.Tweening;
using GameScene.Component;
using GameScene.Component.SelectControl;
using GameScene.Data;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace GameScene
{
    public class PadSelectController : MonoBehaviour
    {
        [Header("For animation")]
        [SerializeField] private Button controlButton;
        [SerializeField] private Transform arrowDir;
        [SerializeField] private Transform selectorPad;
        [SerializeField] private Transform selectedPad;
        [SerializeField] private Transform baseXPositionA;
        [SerializeField] private Transform baseXPositionB;
        private bool isClose;

        [Header("For Controller click and drag")]
        [SerializeField] private GameControlItemGroupData data;
        [SerializeField] private Transform unSelectContainer;
        [SerializeField] private Transform selectedContainer;
        [SerializeField] private Transform movingContainer;
        private List<InteractionItem> storeSelected = new();
        private bool isDelete;
        private const float OffSet = 0.2f;
        [CanBeNull] private InteractionItem selectedObject;

        private void Awake()
        {
            controlButton.onClick.AddListener(OnClickOpenClose);
            CreateSelector();
        }

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
                    HandleMouseMoveSelected();
                }
            }
        }

        public void HandleMouseUp()
        {
            if (!selectedObject)
            {
                return;
            }

            if (storeSelected.Count == 15)
            {
                isDelete = true;
            }

            if (isDelete) // in delete zone
            {
                SimplePool.Despawn(selectedObject!.gameObject);
                selectedObject = null;
                isDelete = false;
            }
            else // Valid pos
            {
                // if (!storeSelected.Contains(selectedObject))
                // {
                //     storeSelected.Insert(CalculatedCurrentPosition(Input.mousePosition), selectedObject);
                // }
                //
                // view.SetParentSelected(selectedObject!.transform);
                // view.ReSortItemsSelected(storeSelected.Select(o => o.RectTransform).ToList());
                // selectedObject = null;
            }
        }

        public void HandleMouseMoveSelected()
        {
            if (!selectedObject)
            {
                return;
            }

            Vector3 mousePos = Input.mousePosition;
            selectedObject!.RectTransform.position = mousePos;

            // handle if inside delete zone
            isDelete = true;
            // isDelete = IsPointInRT(mousePos, deleteZone);
            if (storeSelected.Count == 15)
            {
                isDelete = true;
                return;
            }

            // check to make space
            // HandleDisplayCalculate(mousePos);
        }

        private void CreateSelector()
        {
            // Generate objects selector
            foreach (var o in data.GameControlItemData)
            {
                var obj = Instantiate(data.SelectorModel, unSelectContainer);
                var scriptControl = obj.AddComponent<Arrow>();
                scriptControl.Init(OnClickedSelector);
                scriptControl.SelectType = o.ItemType;
                scriptControl.ChangeRender(o.UnSelectRender);
            }
        }

        #region CALL BACK

        private void OnClickedSelector(InteractionItem selectedObj)
        {
            var obj = SimplePool.Spawn(data.SelectedModel);
            // Generate selected Item
            var arrow = obj.GetComponent<Arrow>();
            arrow.Init(OnClickedSelected);
            arrow.ChangeRender(data.GetByType(selectedObj.SelectType).SelectedRender);
            arrow.SelectType = selectedObj.SelectType;
            // assign to control
            selectedObject = arrow;
            selectedObject.transform.SetParent(movingContainer);
        }

        private void OnClickedSelected(InteractionItem selectedObj)
        {
            // Get object to move
            storeSelected.Remove(selectedObj);
            selectedObject = selectedObj;
            selectedObject!.transform.SetParent(movingContainer);
            // view.ReSortItemsSelected(storeSelected.Select(o => o.RectTransform).ToList());
            //
            // StoreTempPosition();
        }

        #endregion

        #region OPEN/CLOSE Control

        private void OnClickOpenClose()
        {
            controlButton.interactable = false;
            if (isClose)
            {
                isClose = false;
                // Open
                OpenAnimation();
            }
            else
            {
                isClose = true;
                // Close
                CloseAnimation();
            }
        }

        private void OpenAnimation()
        {
            arrowDir.DORotate(new Vector3(0, 0, 0), 0.75f).OnComplete(() => { controlButton.interactable = true; });
            selectorPad.DOMoveX(baseXPositionA.position.x, 0.5f);
            selectedPad.DOMoveX(baseXPositionB.position.x, 0.5f);
        }

        private void CloseAnimation()
        {
            arrowDir.DORotate(new Vector3(0, 180, 0), 0.75f).OnComplete(() => { controlButton.interactable = true; });
            selectorPad.DOMoveX(0, 0.5f);
            selectedPad.DOMoveX(0, 0.5f);
        }

        #endregion
    }
}
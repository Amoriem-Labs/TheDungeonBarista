using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TDB.CraftSystem.UI.RecipeGraph;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TDB.CraftSystem.UI.Inventory
{
    [RequireComponent(typeof(CraftMenuInventoryItemUI))]
    public class CraftMenuInventoryItemInputController : MonoBehaviour, IBeginDragHandler, IDragHandler,
        IEndDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private Transform _dragIcon;
        
        private CraftMenuInventoryItemUI _itemUI;

        // private bool _canQuickAdd = true;
        private bool _quickAddReady = false;
        private GraphicRaycaster _raycaster;
        private Canvas _canvas;

        private void Awake()
        {
            _itemUI = GetComponent<CraftMenuInventoryItemUI>();

            _canvas = GetComponentInParent<Canvas>();
            _raycaster = _canvas.GetComponent<GraphicRaycaster>();
            
            _dragIcon.SetParent(transform.parent);
            _dragIcon.gameObject.SetActive(false);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _quickAddReady = false;
            
            _dragIcon.gameObject.SetActive(true);
            _dragIcon.SetAsLastSibling();
            _dragIcon.position = eventData.pointerCurrentRaycast.worldPosition;
            _dragIcon.DOKill();
        }

        public void OnDrag(PointerEventData eventData)
        {
            _dragIcon.position = eventData.pointerCurrentRaycast.worldPosition;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            List<RaycastResult> results = new();
            _raycaster.Raycast(eventData, results);

            IngredientNodeUI nodeUI = null;
            foreach (RaycastResult result in results)
            {
                if (result.gameObject?.TryGetComponent(out nodeUI) == true) break;
            }

            if (_itemUI.TryAddIngredient(nodeUI, eventData.pointerCurrentRaycast.worldPosition))
            {
                // drag icon is replaced by AddedIngredientIcon
                _dragIcon.gameObject.SetActive(false);
            }
            else
            {
                // animation when failed to add
                _dragIcon.DOKill();
                _dragIcon.DOMove(transform.position, .2f)
                    .SetUpdate(true)
                    .OnComplete(() =>
                    {
                        _dragIcon.gameObject.SetActive(false);
                    });
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _quickAddReady = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            // do nothing if interrupted by dragging
            if (!_quickAddReady) return;
            _quickAddReady = false;

            _itemUI.TryQuickAdd();
        }
    }
}
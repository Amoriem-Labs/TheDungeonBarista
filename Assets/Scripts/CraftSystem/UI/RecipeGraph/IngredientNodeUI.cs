using System;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using TDB.CraftSystem.Data;
using TDB.Utils.EventChannels;
using TDB.Utils.UI.Tooltip;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TDB.CraftSystem.UI.RecipeGraph
{
    public class IngredientNodeUI : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private TextMeshProUGUI _countText;
        [SerializeField] private Image _typeIcon;
        [SerializeField] private CanvasGroup _selectionMarker;

        [TitleGroup("Events")]
        [SerializeField] private EventChannel _onRecipeUpdatedEvent;
        [SerializeField] private EventChannel _returnIngredientEvent;
        [SerializeField] private EventChannel _onNodeSelectionEvent;
        
        private IngredientNodeData _nodeData;
        private AddedIngredientIconControllerPool _addedIngredientPool;
        private readonly List<AddedIngredientIconController> _trackedIngredientIcons = new();
        private bool _selected;

        private TooltipHandler _tooltip;
        
        private string CompleteTooltipText => "Node Completed";
        private string IncompleteTooltipText => "Add {0} Ingredient(s) to Complete The Node";

        private void Awake()
        {
            _addedIngredientPool = GetComponentInParent<AddedIngredientIconControllerPool>();
            _tooltip = GetComponent<TooltipHandler>();

            _selectionMarker.alpha = 0;
        }

        private void OnDisable()
        {
            foreach (var addedIngredient in _trackedIngredientIcons)
            {
                addedIngredient.DestroyObject();
            }
            _trackedIngredientIcons.Clear();

            // broadcast deselection event if selected
            if (_selected)
            {
                _onNodeSelectionEvent.RaiseEvent<IngredientNodeUI>(null);
            }

            _selectionMarker.DOKill();
            _selectionMarker.alpha = 0;
        }

        public void BindNodeData(IngredientNodeData nodeData)
        {
            _nodeData = nodeData;

            // update count
            UpdateCountText();
            // set icon
            _typeIcon.sprite = _nodeData.RequiredType.Icon;
            // added ingredient icons
            foreach (var ingredient in _nodeData.AddedIngredients)
            {
                AddIngredientIconFrom(ingredient, transform.position);
            }
        }

        #region UIUpdates

        private void UpdateCountText()
        {
            _countText.text = $"{_nodeData.AddedIngredients.Count}/{_nodeData.RequiredAmount}";
            _tooltip.SetTooltipText(_nodeData.IsNodeReady
                ? CompleteTooltipText
                : string.Format(IncompleteTooltipText, _nodeData.RequiredAmount - _nodeData.AddedIngredients.Count));
        }

        private void AddIngredientIconFrom(IngredientDefinition ingredient, Vector3 position)
        {
            position.z = transform.position.z;
            var ingredientIcon = _addedIngredientPool.Get(position, Quaternion.identity);
            ingredientIcon.BindData(this, ingredient);
            _trackedIngredientIcons.Add(ingredientIcon);
        }

        #endregion

        #region RecipeManipulation

        public bool TryAddIngredientFrom(IngredientDefinition ingredient, Vector3 position)
        {
            // try to update data
            var success = _nodeData.TryAddIngredient(ingredient);
            if (success)
            {
                // trigger node update events
                _onRecipeUpdatedEvent.RaiseEvent();
                // update UI
                UpdateCountText();
                // add ingredient icon
                AddIngredientIconFrom(ingredient, position);
            }

            return success;
        }

        public void RemoveIngredient(AddedIngredientIconController addedIngredient)
        {
            // update data
            _nodeData.RemoveIngredient(addedIngredient.Ingredient);
            // broadcast event
            _returnIngredientEvent.RaiseEvent(new ReturnIngredientInfo
            {
                Ingredient = addedIngredient.Ingredient,
                Position = addedIngredient.transform.position,
            });
            _onRecipeUpdatedEvent.RaiseEvent();
            // update count
            UpdateCountText();
            // remove added ingredient icon
            _trackedIngredientIcons.Remove(addedIngredient);
            addedIngredient.DestroyObject();
        }
        

        #endregion

        #region InputControl
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (_selected) return;
            
            _onNodeSelectionEvent.RaiseEvent(this);

            _selected = true;
            _onNodeSelectionEvent.AddListener<IngredientNodeUI>(HandleNodeDeselection);

            _selectionMarker.DOKill();
            _selectionMarker.DOFade(1, .2f);
        }

        private void HandleNodeDeselection(IngredientNodeUI node)
        {
            // only handle when deselecting or selecting other nodes
            if (node == this) return;
            
            _selected = false;
            _onNodeSelectionEvent.RemoveListener<IngredientNodeUI>(HandleNodeDeselection);
            
            _selectionMarker.DOKill();
            _selectionMarker.DOFade(0, .5f);
        }

        #endregion
    }

    public struct ReturnIngredientInfo
    {
        public IngredientDefinition Ingredient;
        public Vector3 Position;
    }
}
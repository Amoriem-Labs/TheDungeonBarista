using System;
using System.Collections.Generic;
using TDB.CraftSystem.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TDB.CraftSystem.UI.RecipeGraph
{
    public class IngredientNodeUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _countText;
        [SerializeField] private Image _typeIcon;
        
        private IngredientNodeData _nodeData;
        private AddedIngredientIconControllerPool _addedIngredientPool;
        private readonly List<AddedIngredientIconController> _trackedIngredientIcons = new();

        private void Awake()
        {
            _addedIngredientPool = GetComponentInParent<AddedIngredientIconControllerPool>();
        }

        private void OnDisable()
        {
            foreach (var addedIngredient in _trackedIngredientIcons)
            {
                addedIngredient.DestroyObject();
            }
            _trackedIngredientIcons.Clear();
        }

        public void BindNodeData(IngredientNodeData nodeData)
        {
            _nodeData = nodeData;

            _countText.text = $"{nodeData.AddedIngredient.Count}/{nodeData.RequiredAmount}";
            _typeIcon.sprite = nodeData.RequiredType.Icon;

            foreach (var ingredient in nodeData.AddedIngredient)
            {
                AddIngredientFrom(ingredient, transform.position);
            }
        }

        public void AddIngredientFrom(IngredientDefinition ingredient, Vector3 position)
        {
            position.z = transform.position.z;
            var ingredientIcon = _addedIngredientPool.Get(position, Quaternion.identity);
            ingredientIcon.BindData(this, ingredient);
            _trackedIngredientIcons.Add(ingredientIcon);
        }
    }
}
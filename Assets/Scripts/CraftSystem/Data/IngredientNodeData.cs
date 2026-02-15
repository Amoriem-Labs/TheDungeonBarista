using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TDB.CraftSystem.EffectSystem;
using TDB.CraftSystem.EffectSystem.Data;
using UnityEngine;
using UnityEngine.Serialization;

namespace TDB.CraftSystem.Data
{
    [System.Serializable]
    public class IngredientNodeData
    {
        [FormerlySerializedAs("_addedIngredient")]
        [SerializeField] private List<IngredientDefinition> _addedIngredients;
        [SerializeField] private int _requiredAmount = 3;
        [SerializeField] private IngredientTypeDefinition _requiredType;
        
        [Tooltip("Only in effect when the node is ready.")]
        [TableList(AlwaysExpanded = true), HideLabel, TitleGroup("Effects")]
        [SerializeField] private List<EffectParamPair> _effects;
        
        public IngredientNodeData(IngredientNodeData data)
        {
            _addedIngredients = new(data.AddedIngredients);
            _requiredAmount = data.RequiredAmount;
            _requiredType = data.RequiredType;
            _effects = new List<EffectParamPair>(data._effects);
        }

        public List<IngredientDefinition> AddedIngredients => _addedIngredients;

        public bool IsNodeReady => _addedIngredients.Count >= RequiredAmount;
        public bool IsNodeAvailable => _addedIngredients.Count < RequiredAmount;

        public int RequiredAmount => _requiredAmount;

        public IngredientTypeDefinition RequiredType => _requiredType;

        public List<EffectParamPair> Effects =>
            _addedIngredients.SelectMany(i => i.Effects)
                .Concat(IsNodeReady ? _effects : new List<EffectParamPair>())
                .ToList();

        public void AddIngredient(IngredientDefinition ingredient)
        {
            AddedIngredients.Add(ingredient);
        }

        public void RemoveIngredient(IngredientDefinition ingredient)
        {
            AddedIngredients.Remove(ingredient);
        }

        public bool TryAddIngredient(IngredientDefinition ingredient)
        {
            if (!IsNodeAvailable) return false;
            if (ingredient.Type != _requiredType) return false;
            
            AddIngredient(ingredient);
            return true;
        }
    }
}
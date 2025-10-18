using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TDB.CraftSystem.Data
{
    [System.Serializable]
    public class IngredientNodeData
    {
        [SerializeField] private List<IngredientDefinition> _addedIngredient;
        [SerializeField] private int _requiredAmount = 3;
        [SerializeField] private IngredientTypeDefinition _requiredType;
        
        public IngredientNodeData(IngredientNodeData data)
        {
            _addedIngredient = data.AddedIngredient.ToList();
            _requiredAmount = data.RequiredAmount;
            _requiredType = data.RequiredType;
        }

        public List<IngredientDefinition> AddedIngredient => _addedIngredient;

        public bool IsNodeReady => _addedIngredient.Count >= RequiredAmount;
        public bool IsNodeAvailable => _addedIngredient.Count < RequiredAmount;

        public int RequiredAmount => _requiredAmount;

        public IngredientTypeDefinition RequiredType => _requiredType;

        public void AddIngredient(IngredientDefinition ingredient)
        {
            AddedIngredient.Add(ingredient);
        }

        public void RemoveIngredient(IngredientDefinition ingredient)
        {
            AddedIngredient.Remove(ingredient);
        }
    }
}
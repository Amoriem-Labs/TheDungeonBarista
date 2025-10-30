using System.Linq;
using Sirenix.Serialization;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TDB.CraftSystem.UI.RecipeGraph;
using TDB.Utils.Misc;
using UnityEngine;

[assembly: RegisterFormatter(typeof(RsoPathFormatter<TDB.CraftSystem.Data.RawRecipeDefinition>))]

namespace TDB.CraftSystem.Data
{

    [CreateAssetMenu(fileName = "New Raw Recipe", menuName = "Data/Craft System/Raw Recipe Definition", order = 0)]
    public class RawRecipeDefinition : ResourceScriptableObject
    {
        [SerializeField] private string _recipeName;
        [SerializeField] private RecipeGraphUI _emptyGraphPrefab;
        
        [TableList]
        [SerializeField] private List<IngredientNodeData> _initialNodeData;

        [field:SerializeField] public int BasicPrice { get; private set; } = 100;
        
        // take deep copy of the data
        // all node data in _initialNodeData is a template that should not be changed during runtime
        public List<IngredientNodeData> InitialNodeData => _initialNodeData.Select(n => new IngredientNodeData(n)).ToList();

        public string RecipeName => _recipeName;

        public RecipeGraphUI EmptyGraphPrefab => _emptyGraphPrefab;
    }
}
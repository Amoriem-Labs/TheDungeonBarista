using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TDB.CraftSystem.Data;
using TDB.Utils.EventChannels;
using UnityEngine;

namespace TDB.CraftSystem.UI.RecipeGraph
{
    public class CraftTableUI : MonoBehaviour
    {
        [SerializeField] private Transform _container;
        
        [TitleGroup("Events")]
        [SerializeField] private EventChannel _onRecipeSelectedEvent;

        private RecipeGraphUI _currentGraph;
        private readonly Dictionary<RawRecipeDefinition, RecipeGraphUI> _recipeGraphs = new();
        
        private void OnEnable()
        {
            _onRecipeSelectedEvent.AddListener<FinalRecipeData>(HandleRecipeSelected);
        }

        private void OnDisable()
        {
            _onRecipeSelectedEvent.RemoveListener<FinalRecipeData>(HandleRecipeSelected);
        }

        private void HandleRecipeSelected(FinalRecipeData recipe)
        {
            // disable current graph
            if (_currentGraph) _currentGraph.gameObject.SetActive(false);
            // get or instantiate new graph
            if (!_recipeGraphs.TryGetValue(recipe.RawRecipe, out _currentGraph))
            {
                _currentGraph = Instantiate(recipe.RawRecipe.EmptyGraphPrefab, _container);
                _recipeGraphs.Add(recipe.RawRecipe, _currentGraph);
            }
            _currentGraph.gameObject.SetActive(true);
            // load data
            _currentGraph.LoadRecipeData(recipe);
        }
    }
}
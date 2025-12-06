using System;
using System.Collections.Generic;
using TDB.CraftSystem.Data;
using UnityEngine;

namespace TDB.CraftSystem.UI.RecipeGraph
{
    public class RecipeGraphUI : MonoBehaviour
    {
        [SerializeField] private List<IngredientNodeUI> _ingredientNodes;
        
        public void LoadRecipeData(FinalRecipeData recipe)
        {
            for (int i = 0; i < Mathf.Min(recipe.NodeData.Count, _ingredientNodes.Count); i++)
            {
                var nodeUI = _ingredientNodes[i];
                var nodeData = recipe.NodeData[i];
                nodeUI.BindNodeData(nodeData);
            }
        }
    }
}
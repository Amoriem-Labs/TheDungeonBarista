using System.Collections.Generic;
using UnityEngine;

namespace TDB.CraftSystem.Data
{
    [System.Serializable]
    public class RecipeBookData
    {
        [SerializeField] private List<FinalRecipeData> _allRecipeData;
    }
}
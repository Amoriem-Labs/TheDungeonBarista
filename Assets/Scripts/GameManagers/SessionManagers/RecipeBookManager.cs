using TDB.CraftSystem.Data;
using UnityEngine;

namespace TDB.CafeSystem.Managers
{
    public class RecipeBookManager : MonoBehaviour
    {
        private RecipeBookData _recipeBookData;

        public RecipeBookData RecipeBook
        {
            get
            {
                if (_recipeBookData == null)
                {
                    Debug.LogError("RecipeBookData was accessed before loaded.");
                }
                return _recipeBookData;
            }
        }

        public void Initialize(RecipeBookData recipeBookData)
        {
            _recipeBookData = recipeBookData;
        }

        public RecipeBookData GetRecipeBook() => _recipeBookData;
    }
}

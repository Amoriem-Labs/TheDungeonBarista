using TDB.CraftSystem.Data;
using UnityEngine;

namespace TDB.IngredientStorageSystem.Data
{
    [System.Serializable]
    public class IngredientStorageStackData
    {
        [field: SerializeField]
        public IngredientDefinition Definition { get; private set; }
        [field: SerializeField]
        public int Amount { get; private set; }
    }
}
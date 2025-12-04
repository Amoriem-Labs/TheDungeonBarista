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

        public void Consume(int amount)
        {
            Amount -= amount;
            Amount = Mathf.Max(0, Amount);
        }

        public void Deposit(int amount)
        {
            Amount += amount;
        }

        public IngredientStorageStackData(IngredientDefinition definition)
        {
            Definition = definition;
            Amount = 0;
        }
    }
}
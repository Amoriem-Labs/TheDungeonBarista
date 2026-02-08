using UnityEngine;

namespace TDB.CafeSystem.FurnitureSystem
{
    [CreateAssetMenu(menuName = "Data/Shop/Refrigerator Furniture Definition", fileName = "New Refrigerator Furniture", order = 0)]
    public class RefrigeratorFurnitureDefinition : FurnitureDefinition
    {
        public int Capacity;
    }
}
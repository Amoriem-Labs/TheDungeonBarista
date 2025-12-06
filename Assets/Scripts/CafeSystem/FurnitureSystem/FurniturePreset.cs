using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TDB.CafeSystem.FurnitureSystem
{
    [CreateAssetMenu(fileName = "New Furniture Preset", menuName = "Data/Shop/Furniture Preset", order = 0)]
    public class FurniturePreset : ScriptableObject
    {
        [SerializeField, TableList]
        public List<FurnitureData> FurnitureData;
    }
}
using System;
using System.Text;
using Sirenix.Serialization;
using TDB.CafeSystem.FurnitureSystem;
using TDB.Utils.Misc;
using UnityEngine;

[assembly: RegisterFormatter(typeof(RsoPathFormatter<FurnitureDefinition>))]

namespace TDB.CafeSystem.FurnitureSystem
{
    [CreateAssetMenu(fileName = "New Furniture", menuName = "Data/Shop/Furniture Definition", order = 0)]
    public class FurnitureDefinition : ResourceScriptableObject
    {
        public string DefinitionID;
        public Furniture FurniturePrefab;

    }
}
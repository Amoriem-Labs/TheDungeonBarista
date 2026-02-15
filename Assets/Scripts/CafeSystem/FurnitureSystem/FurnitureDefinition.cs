using System;
using System.Text;
using Sirenix.Serialization;
using TDB.CafeSystem.FurnitureSystem;
using TDB.ShopSystem;
using TDB.ShopSystem.Framework;
using TDB.Utils.Misc;
using UnityEngine;

[assembly: RegisterFormatter(typeof(RsoPathFormatter<FurnitureDefinition>))]

namespace TDB.CafeSystem.FurnitureSystem
{
    [CreateAssetMenu(fileName = "New Furniture", menuName = "Data/Shop/Furniture Definition", order = 0)]
    public class FurnitureDefinition : ResourceScriptableObject, IShopItemDefinition, IPreviewableShopItemDefinition
    {
        public string DefinitionID;
        public string FurnitureName;
        public Furniture FurniturePrefab;

        [field: SerializeField]
        public Sprite PreviewImage { get; private set; }
    }
}
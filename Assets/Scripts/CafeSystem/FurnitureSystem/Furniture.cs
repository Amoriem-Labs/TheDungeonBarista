using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace TDB.CafeSystem.FurnitureSystem
{
    public class Furniture : MonoBehaviour
    {
        [SerializeField, InlineProperty, HideLabel]
        private FurnitureData _data;
        
        private readonly Dictionary<string, IFurniturePartDataHolder> _parts = new();
        public string FurnitureID => _data.FurnitureID;
        public FurnitureDefinition Definition => _data.FurnitureDefinition;

        /// <summary>
        /// Used in place of the Awake method
        /// </summary>
        public void Initialize()
        {
            _parts.Clear();
            foreach (var p in GetComponentsInChildren<IFurniturePartDataHolder>())
            {
                if (!_parts.TryAdd(p.PartID, p))
                {
                    Debug.LogWarning($"Duplicate PartId '{p.PartID}' under {name}. Only the first will be used.");
                }
            }
        }

        #region Data Management

        #region Parts Data

        private List<FurniturePartData> CreateDefaultPartsData() =>
            _parts.Values.Select(p => new FurniturePartData
                    { PartID = p.PartID, Json = DataToJson(p.CreateDefaultData()) })
                .ToList();

        private void LoadPartsData(IEnumerable<FurniturePartData> allPartsData)
        {
            // Index saved parts by PartId for O(1) lookup
            var allPartsDataMap = allPartsData?.ToDictionary(ps => ps.PartID) ??
                                  new Dictionary<string, FurniturePartData>();

            foreach (var (id, part) in _parts)
            {
                if (allPartsDataMap.TryGetValue(id, out var partData))
                {
                    var data = JsonToData(partData.Json, part.DataType);
                    part.LoadData(data);
                }
                else
                {
                    // No saved data → new default (or migration path)
                    var data = part.CreateDefaultData();
                    part.LoadData(data);
                }
            }
        }

        private List<FurniturePartData> ExtractPartsData() =>
            _parts.Values.Select(p => new FurniturePartData
                    { PartID = p.PartID, Json = DataToJson(p.ExtractData()) })
                .ToList();

        #endregion
        
        /// <summary>
        /// Called by FurnitureManager to load furniture data.
        /// </summary>
        /// <param name="data"></param>
        public void LoadData(FurnitureData data)
        {
            _data = data;
            LoadPartsData(data.Parts);
        }

        /// <summary>
        /// Called by Furniture Manager to save furniture data.
        /// </summary>
        /// <returns></returns>
        public FurnitureData ExtractData()
        {
            return new FurnitureData()
            {
                FurnitureID = FurnitureID,
                FurnitureDefinition = Definition,
                Position = transform.position,
                Rotation = transform.rotation,
                Parts = ExtractPartsData()
            };
        }
        
        /// <summary>
        /// Called by Furniture Manager to create default data for new instances.
        /// </summary>
        /// <param name="furnitureID"></param>
        /// <param name="furnitureDefinition"></param>
        /// <returns></returns>
        public FurnitureData CreateNewInstanceData(string furnitureID, FurnitureDefinition furnitureDefinition)
        {
            return new FurnitureData
            {
                FurnitureID = furnitureID,
                FurnitureDefinition = furnitureDefinition,
                Position = transform.position,
                Rotation = transform.rotation,
                Parts = CreateDefaultPartsData()
            };
        }
        
        // --- Serialize any object to JSON text ---
        private static string DataToJson(object data)
        {
            // Weak version accepts System.Object
            var bytes = SerializationUtility.SerializeValueWeak(data, DataFormat.JSON);
            return Encoding.UTF8.GetString(bytes);
        }

        // --- Deserialize back using a runtime Type ---
        private static object JsonToData(string json, Type dataType)
        {
            var bytes = Encoding.UTF8.GetBytes(json);
            // DeserializeValueWeak handles unknown (runtime) types correctly
            return SerializationUtility.DeserializeValueWeak(bytes, DataFormat.JSON);
        }

        #endregion
    }
    
    public interface IFurniturePartDataHolder
    {
        /// Stable ID that won’t change across versions (e.g., "Storage", "PriceTag").
        string PartID { get; }

        /// The concrete data class Type (e.g., typeof(StoragePartData)).
        System.Type DataType { get; }

        /// Return a new default data instance for a freshly placed furniture.
        object CreateDefaultData();

        /// Apply deserialized data to this component (called on load/place).
        void LoadData(object data);

        /// Extract current data (called on save).
        object ExtractData();
    }
}
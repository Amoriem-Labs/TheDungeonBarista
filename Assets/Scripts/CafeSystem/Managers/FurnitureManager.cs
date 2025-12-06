using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TDB.CafeSystem.FurnitureSystem;
using TDB.Utils.DataPersistence;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TDB.CafeSystem.Managers
{
    public class FurnitureManager : MonoBehaviour, IGameDataWriter
    {
        private readonly Dictionary<string, Furniture> _trackedFurnitures = new ();
        private IDataWriterDestination _currentDataDestination;

        private Transform FurnitureRoot => transform;

        private void OnDisable()
        {
            if (_currentDataDestination != null)
            {
                _currentDataDestination.UnregisterDataWriter(this);
                _currentDataDestination = null;
            }
        }

        public void Initialize(List<FurnitureData> furnitureData, IDataWriterDestination dataWriterDestination)
        {
            Clear();

            if (_currentDataDestination != null)
            {
                _currentDataDestination.UnregisterDataWriter(this);
                _currentDataDestination = dataWriterDestination;
                _currentDataDestination.RegisterDataWriter(this);
            }
            
            // all existing furnitures will be removed
            // foreach (var furniture in FurnitureRoot.GetComponentsInChildren<Furniture>())
            // {
            //     _trackedFurnitures.TryAdd(furniture.FurnitureID, furniture);
            //     
            //     // create default data, might be overwritten later
            //     furniture.Initialize();
            //     var data = furniture.CreateNewInstanceData(furniture.FurnitureID, furniture.Definition);
            //     furniture.LoadData(data);
            // }

            foreach (var data in furnitureData)
            {
                // only load transform data and parts data for default existing ones
                // TODO: assuming default existing ones never gets destroyed or upgraded
                if (_trackedFurnitures.TryGetValue(data.FurnitureID, out var furniture))
                {
                    furniture.transform.SetPositionAndRotation(data.Position, data.Rotation);
                    furniture.LoadData(data);
                }
                else
                {
                    var definition = data.FurnitureDefinition;
                    furniture = Instantiate(definition.FurniturePrefab, data.Position, data.Rotation, FurnitureRoot);

                    furniture.Initialize();
                    furniture.LoadData(data);

                    _trackedFurnitures.Add(data.FurnitureID, furniture);
                }
            }
        }
        
        public void PlaceNew(FurnitureDefinition definition, Vector3 position, Quaternion rotation)
        {
            var furniture = Instantiate(definition.FurniturePrefab, position, rotation, FurnitureRoot);
            
            furniture.Initialize();
            var data = furniture.CreateNewInstanceData(definition.DefinitionID + "_" + System.Guid.NewGuid(),
                definition);
            furniture.LoadData(data);

            _trackedFurnitures.Add(furniture.FurnitureID, furniture);
        }
        
        private void Clear()
        {
            foreach (var (_, furniture) in _trackedFurnitures)
            {
                Destroy(furniture.gameObject);
            }
            _trackedFurnitures.Clear();

            // clear dangling furnitures
            foreach (var furniture in FurnitureRoot.GetComponentsInChildren<Furniture>().ToList())
            {
                Destroy(furniture.gameObject);
            }
        }

#if UNITY_EDITOR
        [Button(ButtonSizes.Large)]
        private void ExtractFurniturePreset()
        {
            var allFurnitures = FurnitureRoot.GetComponentsInChildren<Furniture>();
            var furnitureData = allFurnitures.Select(f => f.ExtractData(forceReloadParts: true)).ToList();
            
            // Ask for save path
            var defaultName = $"New Furniture Preset";
            var path = EditorUtility.SaveFilePanelInProject(
                "Save Furniture Preset",
                defaultName,
                "asset",
                "Choose a location and name for the FurniturePreset asset."
            );

            if (string.IsNullOrEmpty(path))
                return; // user cancelled

            // Create or update the asset
            var preset = AssetDatabase.LoadAssetAtPath<FurniturePreset>(path);
            if (preset == null)
            {
                preset = ScriptableObject.CreateInstance<FurniturePreset>();
                preset.FurnitureData = furnitureData;
                AssetDatabase.CreateAsset(preset, path);
                EditorUtility.SetDirty(preset);
            }
            else
            {
                Undo.RecordObject(preset, "Update Furniture Preset");
                preset.FurnitureData = furnitureData;
                EditorUtility.SetDirty(preset);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // Focus / ping for convenience
            Selection.activeObject = preset;
            EditorGUIUtility.PingObject(preset);
        }
#endif
        
        public void WriteToData(GameData data)
        {
            data.AllInstalledFurnitureData = _trackedFurnitures.Select(kv => kv.Value.ExtractData()).ToList();
        }
    }
}
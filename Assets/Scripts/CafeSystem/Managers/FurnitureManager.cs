﻿using System.Collections.Generic;
using TDB.CafeSystem.FurnitureSystem;
using UnityEngine;

namespace TDB.CafeSystem.Managers
{
    public class FurnitureManager : MonoBehaviour
    {
        private readonly Dictionary<string, Furniture> _trackedFurnitures = new ();

        private Transform FurnitureRoot => transform;
        
        public void Initialize(List<FurnitureData> furnitureData)
        {
            Clear();
            
            foreach (var furniture in FurnitureRoot.GetComponentsInChildren<Furniture>())
            {
                _trackedFurnitures.TryAdd(furniture.FurnitureID, furniture);
                
                // create default data, might be overwritten later
                furniture.Initialize();
                var data = furniture.CreateNewInstanceData(furniture.FurnitureID, furniture.Definition);
                furniture.LoadData(data);
            }

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
        }
    }
}
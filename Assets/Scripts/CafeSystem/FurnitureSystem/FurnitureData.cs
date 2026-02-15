using System.Collections.Generic;
using UnityEngine;

namespace TDB.CafeSystem.FurnitureSystem
{
    [System.Serializable]
    public class FurnitureData
    {
        public FurnitureDefinition FurnitureDefinition;
        public string FurnitureID;
        
        [HideInInspector] public Vector3 Position;
        [HideInInspector] public Quaternion Rotation;
     
        [HideInInspector] public List<FurniturePartData> Parts;

        public FurnitureData(FurnitureData furnitureData)
        {
            FurnitureDefinition = furnitureData.FurnitureDefinition;
            FurnitureID = furnitureData.FurnitureID;
            Position = furnitureData.Position;
            Rotation = furnitureData.Rotation;
            Parts = new List<FurniturePartData>(furnitureData.Parts);
        }
        
        public FurnitureData(string furnitureID, FurnitureDefinition furnitureDefinition,
            Vector3 position, Quaternion rotation, List<FurniturePartData> parts)
        {
            FurnitureID = furnitureID;
            FurnitureDefinition = furnitureDefinition;
            Position = position;
            Rotation = rotation;
            Parts = parts;
        }
    }

    [System.Serializable]
    public struct FurniturePartData
    {
        public string PartID;
        public string Json;
    }
}
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
     
        [HideInInspector] public List<FurniturePartData> Parts = new();
    }

    [System.Serializable]
    public class FurniturePartData
    {
        public string PartID;
        public string Json;
    }
}
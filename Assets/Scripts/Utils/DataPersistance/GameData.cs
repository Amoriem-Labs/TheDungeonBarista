using System;
using UnityEngine;

namespace TDB.Utils.DataPersistance
{
    [System.Serializable]
    public class GameData
    {
        public DateTime SaveTime { get; set; }
        
        public GameData()
        {
            
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDB.TileSystem
{

    [CreateAssetMenu(fileName = "TileDatabase", menuName = "TileSystem/TileDatabase")]
    public class TileDatabase : ScriptableObject
    {
        public List<TileDefinition> definitions;

        private static Dictionary<TileType, TileDefinition> map;

        public static void Initialize(TileDatabase db)
        {
            map = new Dictionary<TileType, TileDefinition>();
            foreach (var def in db.definitions)
                map[def.type] = def;
        }

        public static TileDefinition GetDefinition(TileType type)
        {
            if (map == null) Debug.LogError("TileDatabase not initialized!");
            return map.TryGetValue(type, out var def) ? def : null;
        }
    }

}

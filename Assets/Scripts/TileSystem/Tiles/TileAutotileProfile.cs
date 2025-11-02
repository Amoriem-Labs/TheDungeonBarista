using System.Collections.Generic;
using UnityEngine;

namespace TDB.TileSystem
{
    [CreateAssetMenu(fileName = "AutotileProfile", menuName = "TileSystem/Autotile Profile")]
    public class TileAutotileProfile : ScriptableObject
    {
        [System.Serializable]
        public class RuleEntry
        {
            [Tooltip("Bitmask using NESW (1,2,4,8) to define which neighbors are solid")]
            public int mask;
            public Sprite sprite;
            public int rotation;
        }

        public List<RuleEntry> rules;
    }
}

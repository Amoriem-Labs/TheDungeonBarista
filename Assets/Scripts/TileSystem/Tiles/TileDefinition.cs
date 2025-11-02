using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDB.TileSystem
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "TileDefinition", menuName = "TileSystem/TileDefinition")]
    public class TileDefinition : ScriptableObject
    {
        public TileType type;
        public Sprite[] variants;
        public bool isSolid;
    }

}

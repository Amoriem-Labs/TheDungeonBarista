using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDB.TileSystem
{
    [System.Serializable]
    public class TileRule
    {
        [Tooltip("Mask pattern: N=1, E=2, S=4, W=8. Example: 5 = N+S connected.")]
        public int neighborMask;

        public Sprite sprite;
        public int rotation; // degrees
    }

}

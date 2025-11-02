using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TDB.TileSystem
{
    [System.Serializable]
    public class VisualTile
    {
        public Sprite sprite;
        public Quaternion rotation;
        public int variantIndex;
        public TileType type;

        public VisualTile(TileType type, Sprite sprite, Quaternion rotation, int variant = 0)
        {
            this.type = type;
            this.sprite = sprite;
            this.rotation = rotation;
            this.variantIndex = variant;
        }


    }
}
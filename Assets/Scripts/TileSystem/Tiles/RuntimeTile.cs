using UnityEngine;
using UnityEngine.Tilemaps;

namespace TDB.TileSystem
{
    // TileBase can be drawn on Tilemap at runtime
    public class RuntimeTile : TileBase
    {
        public Sprite sprite;
        public Matrix4x4 transform = Matrix4x4.identity;

        //public RuntimeTile(Sprite sprite, Quaternion rotation)
        //{
        //    this.sprite = sprite;
        //    this.transform = Matrix4x4.TRS(Vector3.zero, rotation, Vector3.one);
        //}

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref UnityEngine.Tilemaps.TileData tileData)
        {
            tileData.sprite = sprite;
            tileData.transform = transform;
            tileData.color = Color.white;
            tileData.flags = TileFlags.None;
            tileData.colliderType = Tile.ColliderType.None;
        }
    }
}

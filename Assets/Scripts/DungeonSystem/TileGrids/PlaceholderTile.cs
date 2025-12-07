using UnityEngine;
using UnityEngine.Tilemaps;

namespace TDB.TileGrids
{
    [CreateAssetMenu(menuName = "DualGrid/PlaceholderTile")]
    public class PlaceholderTile : Tile
    {
        public TileType tileType = TileType.None;
    }
}

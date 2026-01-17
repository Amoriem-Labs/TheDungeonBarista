using UnityEngine;
using UnityEngine.Tilemaps;
using TDB.DungeonSystem.Core;
namespace TDB.DungeonSystem.Generate
{
        public class DungeonRenderer : MonoBehaviour
    {
        [SerializeField] private Tilemap tilemap;
  
        public void Render(DungeonGrid grid)
        {
            tilemap.ClearAllTiles();

            for (int x = 0; x < grid.width; x++)
            {
                for (int y = 0; y < grid.height; y++)
                {
                    TileType tile = grid.tiles[x, y];
                    if (tile == null) continue;

                    tilemap.SetTile(
                        new Vector3Int(x, y, 0),
                        tile.visualTile
                    );
                }
            }
        }
    }
}

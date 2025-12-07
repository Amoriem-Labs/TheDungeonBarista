using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TDB.TileGrids
{
    public class GridManager : MonoBehaviour
    {
        // tilemaps
        public Tilemap placeholderTilemap;
        public Tilemap displayTilemap;

        // tile type data
        public TileClass[] tileClasses;

        // lookup by type
        private Dictionary<TileType, TileClass> classLookup;

        private static readonly Vector3Int[] NEIGHBOURS = new Vector3Int[]
        {
        new Vector3Int(0, 0, 0), // TopLeft offset (we adjust later)
        new Vector3Int(1, 0, 0),
        new Vector3Int(0, -1, 0),
        new Vector3Int(1, -1, 0)
        };

        void Start()
        {
            classLookup = new();
            foreach (var tc in tileClasses)
                classLookup[tc.tileType] = tc;

            RefreshDisplayTilemap();
        }

        // Get the TileType stored in placeholder tilemap
        private TileType GetTypeAt(Vector3Int cell)
        {
            PlaceholderTile t = placeholderTilemap.GetTile<PlaceholderTile>(cell);
            if (t == null) return TileType.None;

            // You decide how to map Tile -> TileType
            // Could be via a dictionary too
            foreach (var tc in tileClasses)
            {
                if (tc == null) continue;
                if (tc.name == t.name)  // simple example
                    return tc.tileType;
            }

            return TileType.None;
        }

        public void SetPlaceholder(Vector3Int pos, Tile tile)
        {
            placeholderTilemap.SetTile(pos, tile);
            UpdateDisplayAround(pos);
        }

        private Tile CalculateTile(Vector3Int pos)
        {
            TileType tl = GetTypeAt(pos + new Vector3Int(0, 1, 0));
            TileType tr = GetTypeAt(pos + new Vector3Int(1, 1, 0));
            TileType bl = GetTypeAt(pos + new Vector3Int(0, 0, 0));
            TileType br = GetTypeAt(pos + new Vector3Int(1, 0, 0));

            // The tile we are *rendering* is based on the bottom-left cell's tile type
            TileType baseType = bl;

            if (!classLookup.TryGetValue(baseType, out TileClass tc))
                return null;

            return tc.GetTileForNeighbours(tl, tr, bl, br);
        }

        private void UpdateDisplayAround(Vector3Int pos)
        {
            // update the 4 cells affected
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    Vector3Int p = pos + new Vector3Int(i, j, 0);
                    displayTilemap.SetTile(p, CalculateTile(p));
                }
            }
        }

        public void RefreshDisplayTilemap()
        {
            for (int x = -50; x < 50; x++)
                for (int y = -50; y < 50; y++)
                {
                    Vector3Int c = new Vector3Int(x, y, 0);
                    displayTilemap.SetTile(c, CalculateTile(c));
                }
        }
    }
}
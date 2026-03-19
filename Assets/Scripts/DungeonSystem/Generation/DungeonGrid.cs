using UnityEngine;

namespace TDB.DungeonSystem.Core
{
    public class DungeonGrid
    {
        public TileType[,] tiles;
        public TileType[,] decorationTiles;
        public int width;
        public int height;

        public DungeonGrid(int width, int height)
        {
            this.width = width;
            this.height = height;
            tiles = new TileType[width, height];
            decorationTiles = new TileType[width, height];
        }

        public void SetTile(Vector2Int pos, TileType tile)
        {
            if (InBounds(pos))
                tiles[pos.x, pos.y] = tile;
        }

        public TileType GetTile(Vector2Int pos)
        {
            if (!InBounds(pos)) return null;
            return tiles[pos.x, pos.y];
        }

        public void SetDecoration(Vector2Int pos, TileType tile)
        {
            if (InBounds(pos))
                decorationTiles[pos.x, pos.y] = tile;
        }

        public TileType GetDecoration(Vector2Int pos)
        {
            if (!InBounds(pos)) return null;
            return decorationTiles[pos.x, pos.y];
        }

        public bool InBounds(Vector2Int pos)
        {
            return pos.x >= 0 && pos.y >= 0 &&
                   pos.x < width && pos.y < height;
        }
    }
}

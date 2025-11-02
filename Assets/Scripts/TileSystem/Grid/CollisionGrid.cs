using System.Collections.Generic;
using TDB;
using UnityEngine;

namespace TDB.TileSystem
{
    public class CollisionGrid
    {
        private TileType[,] grid;
        public int width, height;

        public CollisionGrid(int width, int height)
        {
            this.width = width;
            this.height = height;
            grid = new TileType[width, height];
        }

        public TileType GetTile(int x, int y)
        {
            if (x < 0 || y < 0 || x >= width || y >= height) return TileType.Empty;
            return grid[x, y];
        }

        public void SetTile(int x, int y, TileType type)
        {
            if (x < 0 || y < 0 || x >= width || y >= height) return;
            grid[x, y] = type;
        }

        public IEnumerable<Vector2Int> GetNeighbors(Vector2Int pos)
        {
            Vector2Int[] dirs = {
            new Vector2Int(0,1), new Vector2Int(1,0),
            new Vector2Int(0,-1), new Vector2Int(-1,0)
        };
            foreach (var d in dirs)
                yield return pos + d;
        }
    }
}
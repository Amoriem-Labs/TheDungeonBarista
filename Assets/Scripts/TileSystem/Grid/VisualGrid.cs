using System.Collections;
using System.Collections.Generic;
using TDB.TileSystem;
using UnityEngine;
namespace TDB.TileSystem
{
    public class VisualGrid
    {
        private VisualTile[,] grid;
        private int width, height;
        private float offset = 0.5f;

        public VisualGrid(int width, int height)
        {
            this.width = width;
            this.height = height;
            grid = new VisualTile[width, height];
        }

        public void UpdateFromCollisionGrid(CollisionGrid collision)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    grid[x, y] = DetermineVisualTile(collision, x, y);
                }
            }
        }

        private VisualTile DetermineVisualTile(CollisionGrid col, int x, int y)
        {
            TileType type = col.GetTile(x, y);
            if (type == TileType.Empty) return null;

            TileDefinition def = TileDatabase.GetDefinition(type);
            return TileAutotiler.GetVisualTile(col, x, y, def);
        }
        public VisualTile GetVisualTileAt(int x, int y)
        {
            if (x < 0 || y < 0 || x >= width || y >= height) return null;
            return grid[x, y];
        }




    }
}

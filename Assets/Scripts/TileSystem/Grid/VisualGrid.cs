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
        private TileType ChooseDominantTileType(CollisionGrid col, int x, int y)
        {
            // corners: NW=(x,y+1), NE=(x+1,y+1), SE=(x+1,y), SW=(x,y)
            TileType[] corners = new TileType[4];
            corners[0] = col.GetTile(x, y + 1); // NW
            corners[1] = col.GetTile(x + 1, y + 1); // NE
            corners[2] = col.GetTile(x + 1, y); // SE
            corners[3] = col.GetTile(x, y); // SW

            // count occurrences
            var counts = new Dictionary<TileType, int>();
            foreach (var t in corners)
            {
                if (counts.ContainsKey(t)) counts[t]++; else counts[t] = 1;
            }

            // choose the tile type with highest count; tie -> prefer non-empty
            TileType best = TileType.Empty;
            int bestCount = -1;
            foreach (var kv in counts)
            {
                int c = kv.Value;
                TileType k = kv.Key;
                if (c > bestCount || (c == bestCount && k != TileType.Empty && best == TileType.Empty))
                {
                    best = k;
                    bestCount = c;
                }
            }

            return best;
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

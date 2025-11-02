using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDB.TileSystem
{
    public static class TileAutotiler
    {
        private static Dictionary<int, TileRule> rules = new Dictionary<int, TileRule>();

        // Initialize rules for testing
        static TileAutotiler()
        {
            //To-do: UPDATE
            AddRule(0b0000, null, 0); // isolated
            AddRule(0b1111, null, 0); // filled
            AddRule(0b0101, null, 0); // vertical
            AddRule(0b1010, null, 90); // horizontal
            AddRule(0b1000, null, 0); // wall on top
        }

        public static void AddRule(int mask, Sprite sprite, int rotation)
        {
            rules[mask] = new TileRule
            {
                neighborMask = mask,
                sprite = sprite,
                rotation = rotation
            };
        }

        public static VisualTile GetVisualTile(CollisionGrid grid, int x, int y, TileDefinition def)
        {
            if (def == null)
            {
                Debug.LogError($"Tile Definition is null at {x}, {y}");
                return null;
            }

            bool IsNeighborSolid(int nx, int ny)
            {
                if (nx < 0 || ny < 0 || nx >= grid.width || ny >= grid.height)
                    return false;
                return grid.GetTile(nx, ny).IsSolid();
            }

            bool n = IsNeighborSolid(x, y + 1);
            bool e = IsNeighborSolid(x + 1, y);
            bool s = IsNeighborSolid(x, y - 1);
            bool w = IsNeighborSolid(x - 1, y);


            int mask = (n ? 1 : 0) | (e ? 2 : 0) | (s ? 4 : 0) | (w ? 8 : 0);

            if (!rules.TryGetValue(mask, out var rule))
            {
                // Default fallback
                Sprite fallback = (def.variants != null && def.variants.Length > 0) ? def.variants[0] : null;
                return new VisualTile(def.type, fallback, Quaternion.identity);

            }

            Sprite sprite = rule.sprite ?? def.variants[0];
            Quaternion rot = Quaternion.Euler(0, 0, rule.rotation);
            return new VisualTile(def.type, sprite, rot);
        }
    }


}

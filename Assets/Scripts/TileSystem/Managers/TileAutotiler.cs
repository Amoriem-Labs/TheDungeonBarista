using System.Collections.Generic;
using UnityEngine;

namespace TDB.TileSystem
{
    public static class TileAutotiler
    {
        private static Dictionary<int, TileRule> rules = new Dictionary<int, TileRule>();

        public static void Initialize(TileAutotileProfile profile)
        {
            rules.Clear();
            if (profile == null)
            {
                Debug.LogWarning("No autotile profile assigned — using fallback behavior.");
                return;
            }

            foreach (var entry in profile.rules)
            {
                if (entry.sprite == null)
                    Debug.LogError($"Rule mask {entry.mask} has null sprite!");
                rules[entry.mask] = new TileRule
                {
                    neighborMask = entry.mask,
                    sprite = entry.sprite,
                    rotation = entry.rotation
                };
                //Debug.Log($"Adding rule mask {entry.mask} with sprite {entry.sprite.}");
                //rules[entry.mask] = new TileRule
                //{
                //    neighborMask = entry.mask,
                //    sprite = entry.sprite,
                //    rotation = entry.rotation
                //};

            }
        }

        public static VisualTile GetVisualTile(CollisionGrid grid, int x, int y, TileDefinition def)
        {
            if (def == null)
            {
                Debug.LogError($"Tile Definition is null at {x}, {y}");
                return null;
            }

            bool IsCollisionSolid(int cx, int cy)
            {
                if (cx < 0 || cy < 0 || cx >= grid.width || cy >= grid.height) return false;
                return grid.GetTile(cx, cy).IsSolid();
            }

            bool nw = IsCollisionSolid(x, y + 1);
            bool ne = IsCollisionSolid(x + 1, y + 1);
            bool se = IsCollisionSolid(x + 1, y);
            bool sw = IsCollisionSolid(x, y);


            int mask = (nw ? 1 : 0) | (ne ? 2 : 0) | (se ? 4 : 0) | (sw ? 8 : 0);
            Debug.Log($"Tile at ({x},{y}): computed mask={mask}, rule found? {rules.ContainsKey(mask)}");

            Debug.Log($"Mask at {x},{y} = {mask}, found rule? {rules.ContainsKey(mask)}");
            if (!rules.TryGetValue(mask, out var rule))
            {
                Sprite fallback = (def.variants != null && def.variants.Length > 0) ? def.variants[0] : null;
                return new VisualTile(def.type, fallback, Quaternion.identity);
            }

            Sprite sprite = rule.sprite;
            if (sprite == null)
            {
                sprite = (def.variants != null && def.variants.Length > 0) ? def.variants[0] : null;
            }
            Quaternion rot = Quaternion.Euler(0, 0, rule.rotation);
            return new VisualTile(def.type, sprite, rot);
        }
    }
    }

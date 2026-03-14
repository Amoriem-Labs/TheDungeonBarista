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

                    Vector3Int cell = new Vector3Int(x, y, 0);
                    tilemap.SetTransformMatrix(cell, Matrix4x4.identity);

                    if (TryResolveAutoTile(grid, x, y, tile, out TileBase resolvedTile, out int rotationSteps))
                    {
                        tilemap.SetTile(cell, resolvedTile);

                        if (rotationSteps != 0)
                        {
                            float degrees = -90f * rotationSteps;
                            tilemap.SetTransformMatrix(cell, Matrix4x4.Rotate(Quaternion.Euler(0f, 0f, degrees)));
                        }

                        continue;
                    }

                    tilemap.SetTile(cell, tile.visualTile);
                }
            }
        }

        private bool TryResolveAutoTile(DungeonGrid grid, int x, int y, TileType tile, out TileBase resolvedTile, out int rotationSteps)
        {
            resolvedTile = null;
            rotationSteps = 0;

            if (tile == null || !tile.useRuleSet || tile.ruleSet == null)
                return false;

            TileNeighborMask neighborMask = BuildNeighborMask(grid, x, y, tile);
            if (!tile.ruleSet.TryMatch(neighborMask, out TileRule matchedRule, out rotationSteps))
                return false;

            resolvedTile = matchedRule.outputTile;
            return true;
        }

        private TileNeighborMask BuildNeighborMask(DungeonGrid grid, int x, int y, TileType centerTile)
        {
            TileNeighborMask mask = TileNeighborMask.None;

            if (IsMatched(grid, x, y + 1, centerTile)) mask |= TileNeighborMask.North;
            if (IsMatched(grid, x + 1, y, centerTile)) mask |= TileNeighborMask.East;
            if (IsMatched(grid, x, y - 1, centerTile)) mask |= TileNeighborMask.South;
            if (IsMatched(grid, x - 1, y, centerTile)) mask |= TileNeighborMask.West;

            if (IsMatched(grid, x + 1, y + 1, centerTile)) mask |= TileNeighborMask.NorthEast;
            if (IsMatched(grid, x + 1, y - 1, centerTile)) mask |= TileNeighborMask.SouthEast;
            if (IsMatched(grid, x - 1, y - 1, centerTile)) mask |= TileNeighborMask.SouthWest;
            if (IsMatched(grid, x - 1, y + 1, centerTile)) mask |= TileNeighborMask.NorthWest;

            return mask;
        }

        private bool IsMatched(DungeonGrid grid, int x, int y, TileType centerTile)
        {
            TileType neighbor = grid.GetTile(new Vector2Int(x, y));
            if (neighbor == null) return false;

            switch (centerTile.neighborMatchMode)
            {
                case TileNeighborMatchMode.AnyNonNull:
                    return true;
                case TileNeighborMatchMode.AnyWalkable:
                    return neighbor.walkable;
                case TileNeighborMatchMode.SameTileType:
                default:
                    return neighbor == centerTile;
            }
        }
    }
}

using UnityEngine;
using UnityEngine.Tilemaps;
using TDB.DungeonSystem.Core;

namespace TDB.DungeonSystem.Generate
{
    public class DungeonRenderer : MonoBehaviour
    {
        [SerializeField] private Tilemap tilemap;
        [SerializeField] private Tilemap decorationTilemap;
        [SerializeField] private bool buildColliders = true;
        [SerializeField] private bool useCompositeCollider = true;
        [SerializeField] private string collisionContainerName = "_TileColliders";
        [SerializeField] private bool buildDecorationColliders = true;
        [SerializeField] private string decorationCollisionContainerName = "_DecorationTileColliders";
        [SerializeField] private LayerMask collisionLayerName;
        [SerializeField] private Tilemap backgroundTilemap;
        [SerializeField] private TileBase waterTile; 
        [SerializeField] private int borderPadding = 5;
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

            RenderDecorations(grid);

            if (buildColliders)
            {
                BuildTileColliders(grid);
            }
            if (buildDecorationColliders)
            {
                BuildDecorationColliders(grid);
            }
            FillBackground(grid);

        }
        private void FillBackground(DungeonGrid grid)
        {
        backgroundTilemap.ClearAllTiles();

            for (int x = -borderPadding; x < grid.width + borderPadding; x++)
            {
                for (int y = -borderPadding; y < grid.height + borderPadding; y++)
                {
                    Vector3Int cell = new Vector3Int(x, y, 0);
                    backgroundTilemap.SetTile(cell, waterTile);
                }
            }
        }

        private void RenderDecorations(DungeonGrid grid)
        {
            if (decorationTilemap == null || grid == null) return;

            decorationTilemap.ClearAllTiles();
            if (decorationTilemap.TryGetComponent(out TilemapRenderer renderer))
            {
                renderer.mode = TilemapRenderer.Mode.Individual;
                renderer.sortOrder = TilemapRenderer.SortOrder.TopLeft;
            }

            for (int x = 0; x < grid.width; x++)
            {
                for (int y = 0; y < grid.height; y++)
                {
                    TileType tile = grid.decorationTiles[x, y];
                    if (tile == null) continue;

                    Vector3Int cell = new Vector3Int(x, y, 0);
                    decorationTilemap.SetTransformMatrix(cell, Matrix4x4.identity);
                    decorationTilemap.SetTile(cell, tile.visualTile);
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

        public Vector3 GetCellCenterWorld(Vector2Int cell)
        {
            return tilemap.GetCellCenterWorld(new Vector3Int(cell.x, cell.y, 0));
        }

        public Vector3 GetCellCenterWorld(Vector3Int cell)
        {
            return tilemap.GetCellCenterWorld(cell);
        }

        private void BuildTileColliders(DungeonGrid grid)
        {
            if (tilemap == null || grid == null) return;

            Transform existing = tilemap.transform.Find(collisionContainerName);
            if (existing != null)
            {
                if (Application.isPlaying)
                    Destroy(existing.gameObject);
                else
                    DestroyImmediate(existing.gameObject);
            }

            GameObject container = new GameObject(collisionContainerName);
            container.transform.SetParent(tilemap.transform, false);
    
            container.layer = Mathf.RoundToInt(Mathf.Log(collisionLayerName.value, 2)); 

            CompositeCollider2D composite = null;
            if (useCompositeCollider)
            {
                Rigidbody2D rb = container.AddComponent<Rigidbody2D>();
                rb.bodyType = RigidbodyType2D.Static;
                composite = container.AddComponent<CompositeCollider2D>();
                composite.geometryType = CompositeCollider2D.GeometryType.Polygons;
            }

            for (int x = 0; x < grid.width; x++)
            {
                for (int y = 0; y < grid.height; y++)
                {
                    TileType tile = grid.tiles[x, y];
                    if (tile == null || tile.walkable) continue;

                    Vector3Int cell = new Vector3Int(x, y, 0);
                    Vector3 centerLocal = tilemap.GetCellCenterLocal(cell);

                    GameObject colliderObj = new GameObject($"Collider_{x}_{y}");
                    colliderObj.transform.SetParent(container.transform, false);
                    colliderObj.transform.localPosition = centerLocal;
                    colliderObj.layer = container.layer;

                    BoxCollider2D box = colliderObj.AddComponent<BoxCollider2D>();
                    box.size = new Vector2(tilemap.cellSize.x, tilemap.cellSize.y);
                    if (composite != null)
                    {
                        box.usedByComposite = true;
                    }
                }
            }

            Physics2D.SyncTransforms();
        }

        private void BuildDecorationColliders(DungeonGrid grid)
        {
            if (decorationTilemap == null || grid == null) return;

            Transform existing = decorationTilemap.transform.Find(decorationCollisionContainerName);
            if (existing != null)
            {
                if (Application.isPlaying)
                    Destroy(existing.gameObject);
                else
                    DestroyImmediate(existing.gameObject);
            }

            GameObject container = new GameObject(decorationCollisionContainerName);
            container.transform.SetParent(decorationTilemap.transform, false);
            container.layer = Mathf.RoundToInt(Mathf.Log(collisionLayerName.value, 2));

            CompositeCollider2D composite = null;
            if (useCompositeCollider)
            {
                Rigidbody2D rb = container.AddComponent<Rigidbody2D>();
                rb.bodyType = RigidbodyType2D.Static;
                composite = container.AddComponent<CompositeCollider2D>();
                composite.geometryType = CompositeCollider2D.GeometryType.Polygons;
            }

            for (int x = 0; x < grid.width; x++)
            {
                for (int y = 0; y < grid.height; y++)
                {
                    TileType tile = grid.decorationTiles[x, y];
                    if (tile == null || tile.walkable) continue;

                    Vector3Int cell = new Vector3Int(x, y, 0);
                    Vector3 centerLocal = decorationTilemap.GetCellCenterLocal(cell);

                    GameObject colliderObj = new GameObject($"DecoCollider_{x}_{y}");
                    colliderObj.transform.SetParent(container.transform, false);
                    colliderObj.transform.localPosition = centerLocal;
                    colliderObj.layer = container.layer;

                    BoxCollider2D box = colliderObj.AddComponent<BoxCollider2D>();
                    box.size = new Vector2(decorationTilemap.cellSize.x, decorationTilemap.cellSize.y);
                    if (composite != null)
                    {
                        box.usedByComposite = true;
                    }
                }
            }

            Physics2D.SyncTransforms();
        }
    }
}

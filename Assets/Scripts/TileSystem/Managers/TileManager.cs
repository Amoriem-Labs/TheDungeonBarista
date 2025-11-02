using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
namespace TDB.TileSystem
{
    public class TileManager : MonoBehaviour
    {
        public int width = 32, height = 32;
        public CollisionGrid collisionGrid;
        public VisualGrid visualGrid;
        public TileDatabase tileDB;
        public Tilemap visualTilemap;
        public TileAutotileProfile autotileProfile;
        void Start()
        {
            if (tileDB == null) Debug.LogError("TileManager: tileDB is not assigned!");

            TileDatabase.Initialize(tileDB);
            collisionGrid = new CollisionGrid(width, height);
            visualGrid = new VisualGrid(width, height);
            TileAutotiler.Initialize(autotileProfile);
            
            //Test 
            GenerateTestLayout();
            RefreshVisuals();
            RenderVisualGrid();
        }

        void GenerateTestLayout()
        {
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    collisionGrid.SetTile(x, y, (x > 2 && y > 2) ? TileType.Floor : TileType.Empty);

        }

        public void RefreshVisuals()
        {
            visualGrid.UpdateFromCollisionGrid(collisionGrid);
        }

        void RenderVisualGrid()
        {
            if(visualTilemap == null)
            {
                Debug.LogError("TileManager.RenderVisualGrid: visualTilemap reference is missing.");
                return;
            }

            visualTilemap.ClearAllTiles();

            for (int x = 0;x < width; x++)
            {
                for(int y = 0;  y < height; y++)
                {
                    VisualTile vTile = visualGrid.GetVisualTileAt(x, y);
                    if (vTile == null) continue;

                    if (vTile.sprite == null)
                    {
                        Debug.Log($"Tile at {x},{y} has no sprite; skipping render.");
                        continue;
                    }
                    RuntimeTile runtimeTile = ScriptableObject.CreateInstance<RuntimeTile>();
                    runtimeTile.sprite = vTile.sprite;
                    runtimeTile.transform = Matrix4x4.TRS(Vector3.zero, vTile.rotation, Vector3.one);
                    visualTilemap.SetTile(new Vector3Int(x, y, 0), runtimeTile);

                    //var tile = ScriptableObject.CreateInstance<Tile>();
                    //tile.sprite = vTile.sprite;


                    //visualTilemap.SetTile(new Vector3Int(x, y, 0), tile);
                    //visualTilemap.SetTransformMatrix(new Vector3Int(x, y, 0),
                    //    Matrix4x4.TRS(Vector3.zero, vTile.rotation, Vector3.one));

                }
            }
        }
        public int GetCornerMaskAt(int x, int y)
        {
            bool IsSolid(int cx, int cy)
            {
                if (collisionGrid == null) return false;
                if (cx < 0 || cy < 0 || cx >= collisionGrid.width || cy >= collisionGrid.height) return false;
                return collisionGrid.GetTile(cx, cy).IsSolid();
            }

            bool nw = IsSolid(x, y + 1);
            bool ne = IsSolid(x + 1, y + 1);
            bool se = IsSolid(x + 1, y);
            bool sw = IsSolid(x, y);

            int mask = (nw ? 1 : 0) | (ne ? 2 : 0) | (se ? 4 : 0) | (sw ? 8 : 0);
            return mask;
        }

    }

}

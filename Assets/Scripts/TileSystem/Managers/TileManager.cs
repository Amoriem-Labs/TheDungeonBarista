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

        void Start()
        {
            TileDatabase.Initialize(tileDB);
            collisionGrid = new CollisionGrid(width, height);
            visualGrid = new VisualGrid(width, height);

            //Test 
            GenerateTestLayout();
            RefreshVisuals();
            RenderVisualGrid();
        }

        void GenerateTestLayout()
        {
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    collisionGrid.SetTile(x, y, TileType.Floor);
        }

        public void RefreshVisuals()
        {
            visualGrid.UpdateFromCollisionGrid(collisionGrid);
        }

        void RenderVisualGrid()
        {
            visualTilemap.ClearAllTiles();

            for (int x = 0;x < width; x++)
            {
                for(int y = 0;  y < height; y++)
                {
                    VisualTile vTile = visualGrid.GetVisualTileAt(x, y);
                    if (vTile == null) continue;

                    var tile = ScriptableObject.CreateInstance<Tile>();
                    tile.sprite = vTile.sprite;
                    if (vTile == null)
                    {
                        Debug.Log($"Empty visual tile at {x},{y}");
                        continue;
                    }

                    if (vTile.sprite == null)
                    {
                        Debug.Log($"Tile at {x},{y} has no sprite");
                        continue;
                    }

                    visualTilemap.SetTile(new Vector3Int(x, y, 0), tile);
                    visualTilemap.SetTransformMatrix(new Vector3Int(x, y, 0),
                    Matrix4x4.TRS(Vector3.zero, vTile.rotation, Vector3.one));

                }
            }
        }
    }

}

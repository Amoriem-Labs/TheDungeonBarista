using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.GraphView.GraphView;

namespace TDB.Tilemaps
{
    public class DualGridRenderer : MonoBehaviour
    {
        // Initializ the two Tilemaps
        public Tilemap visualTilemap;
        public Tilemap collisionTilemap;
        public TileBase[] visualTiles;
        public TileBase collisionTile;

        //Place a collision tile at position pos and update the visual tile
        public void PlaceTile(Vector3Int pos)
        {
            // Place the collision tile
            collisionTilemap.SetTile(pos, collisionTile);

            // Update visual tiles around this collision tile
            UpdateVisualAtCollision(pos);
            UpdateVisualAtCollision(pos + Vector3Int.left);
            UpdateVisualAtCollision(pos + Vector3Int.right);
            UpdateVisualAtCollision(pos + Vector3Int.up);
            UpdateVisualAtCollision(pos + Vector3Int.down);
        }

        void UpdateVisualAtCollision(Vector3Int visualPos)
        {
            // Define the 4 collision positions under for this visual tile
            Vector3Int[] collisionPositions = new Vector3Int[]
            {
                visualPos,
                visualPos + Vector3Int.right,
                visualPos + Vector3Int.up,
                visualPos + Vector3Int.right + Vector3Int.up
            };
            
            // Check which of the 4 collision tiles are present
            //
            bool[] filled = new bool[4];
            for (int i = 0; i < 4; i++)
            {
                filled[i] = collisionTilemap.GetTile(collisionPositions[i]) != null;
            }
            //set the visual tile
            int tileIndex = GetTileIndex(filled);
            visualTilemap.SetTile(visualPos, visualTiles[tileIndex]);
        }

        int GetTileIndex(bool[] filled)
        {
            // Make binary mask from bools
            int mask = 0;
            mask |= (filled[2] ? 1 << 3 : 0); // top-left sets bit 3
            mask |= (filled[3] ? 1 << 2 : 0); // top-right sets bit 2
            mask |= (filled[0] ? 1 << 1 : 0); // bottom-left sets bit 1
            mask |= (filled[1] ? 1 << 0 : 0); // bottom-right sets bit 0

            // Map mask to visual tile index (0–15)
            switch (mask)
            {
                case 0b0000: return 0;  // empty
                case 0b0001: return 1;  // bottom-right only
                case 0b0010: return 2;  // bottom-left only
                case 0b0011: return 3;  // bottom row
                case 0b0100: return 4;  // top-right only
                case 0b0101: return 5;  // diagonal TR-BR
                case 0b0110: return 6;  // diagonal TL-BR
                case 0b0111: return 7;  // top-right + bottom row
                case 0b1000: return 8;  // top-left only
                case 0b1001: return 9;  // TL + BR
                case 0b1010: return 10; // TL + BL
                case 0b1011: return 11; // TL + bottom row
                case 0b1100: return 12; // top row
                case 0b1101: return 13; // TR + bottom-left + bottom-right
                case 0b1110: return 14; // TL + top-right + bottom-left
                case 0b1111: return 15; // full tile
                default: return 0;
            }
        }

    }
}

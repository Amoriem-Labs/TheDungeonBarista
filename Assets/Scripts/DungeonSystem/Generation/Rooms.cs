using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TDB/Dungeon/Room")]
public class RoomSO : ScriptableObject
{
    public int width;
    public int height;

    // Flattened tile array: index = x + y * width
    public TileType[] tiles;

    public List<Vector2Int> doorPositions = new();

    private void OnValidate()
    {
        if (width <= 0 || height <= 0)
            return;

        int expectedSize = width * height;

        if (tiles == null || tiles.Length != expectedSize)
        {
            tiles = new TileType[expectedSize];
        }

    }

}

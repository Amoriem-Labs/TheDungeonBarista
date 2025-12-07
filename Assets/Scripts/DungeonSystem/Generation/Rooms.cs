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
}

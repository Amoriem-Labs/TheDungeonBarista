using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TDB/Dungeon/Room")]
public class RoomSO : ScriptableObject
{
    public int width;
    public int height;

    // Flattened tile array: index = x + y * width
    public TileType[] tiles;
    [SerializeField] private TileType defaultTile;
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

        // Fill empty slots with defaultTile
        for (int i = 0; i < tiles.Length; i++)
        {
            if (tiles[i] == null)
                tiles[i] = defaultTile;
        }

    }

    public Vector2Int GetRandomDoorWorld(RectInt placedRoom)
    {
        if (doorPositions.Count == 0)
            return new Vector2Int(
                placedRoom.x + placedRoom.width / 2,
                placedRoom.y + placedRoom.height / 2
            );

        Vector2Int local = doorPositions[Random.Range(0, doorPositions.Count)];
        return new Vector2Int(
            placedRoom.x + local.x,
            placedRoom.y + local.y
        );
    }


}

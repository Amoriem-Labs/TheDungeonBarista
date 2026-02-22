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
    public List<Vector2Int> doorPositions = new List<Vector2Int>();

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

        if (doorPositions == null)
            doorPositions = new List<Vector2Int>();

        // Keep only in-bounds perimeter doors so corridors connect at room edges.
        for (int i = doorPositions.Count - 1; i >= 0; i--)
        {
            if (!IsDoorOnPerimeter(doorPositions[i]))
                doorPositions.RemoveAt(i);
        }
    }

    public Vector2Int GetRandomDoorWorld(RectInt placedRoom)
    {
        List<Vector2Int> worldDoors = GetDoorWorldPositions(placedRoom);
        return worldDoors[Random.Range(0, worldDoors.Count)];
    }

    public List<Vector2Int> GetDoorWorldPositions(RectInt placedRoom)
    {
        List<Vector2Int> worldDoors = new List<Vector2Int>();

        if (doorPositions != null)
        {
            foreach (Vector2Int local in doorPositions)
            {
                if (!IsDoorOnPerimeter(local))
                    continue;

                worldDoors.Add(new Vector2Int(placedRoom.x + local.x, placedRoom.y + local.y));
            }
        }

        if (worldDoors.Count == 0)
        {
            worldDoors.Add(new Vector2Int(
                placedRoom.x + placedRoom.width / 2,
                placedRoom.y + placedRoom.height / 2
            ));
        }

        return worldDoors;
    }

    private bool IsDoorOnPerimeter(Vector2Int local)
    {
        if (local.x < 0 || local.y < 0 || local.x >= width || local.y >= height)
            return false;

        bool onLeftOrRightEdge = local.x == 0 || local.x == width - 1;
        bool onTopOrBottomEdge = local.y == 0 || local.y == height - 1;
        return onLeftOrRightEdge || onTopOrBottomEdge;
    }
}

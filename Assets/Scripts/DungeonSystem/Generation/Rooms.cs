using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "TDB/Dungeon/Room")]
public class RoomSO : ScriptableObject
{
    [System.Serializable]
    public class DecorationTile
    {
        public Vector2Int Position;
        public TileType Tile;
        [Range(0, 1)]
        public float Probability;
    }
    
    [Header("Classification")]
    public RoomType roomType = RoomType.Normal;

    [Min(1)]
    public int width;
    [Min(1)]
    public int height;

    // Flattened tile array: index = x + y * width
    public TileType[] tiles;
    [Header("Theme")]
    public TileType wallTile;
    public TileType corridorFloorTile;
    [SerializeField] private TileType defaultTile;
    [FormerlySerializedAs("decorationTiles")] [Header("Decoration")]
    public TileType[] decorationTiles_deprecated;

    public List<DecorationTile> decorations = new List<DecorationTile>();

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

        // if (decorationTiles == null || decorationTiles.Length != expectedSize)
        // {
        //     decorationTiles = new TileType[expectedSize];
        // }

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
        
        if (decorations == null)
            return;

        for (int i = 0; i < decorations.Count; i++)
        {
            var d = decorations[i];
            if (d == null)
                continue;

            d.Position = new Vector2Int(
                Mathf.Clamp(d.Position.x, 0, width - 1),
                Mathf.Clamp(d.Position.y, 0, height - 1)
            );
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

    public TileType GetCorridorFloorTile()
    {
        if (corridorFloorTile != null)
            return corridorFloorTile;

        if (defaultTile != null)
            return defaultTile;

        if (tiles == null)
            return null;

        TileType fallback = null;
        for (int i = 0; i < tiles.Length; i++)
        {
            TileType tile = tiles[i];
            if (tile == null) continue;
            if (tile.walkable)
                return tile;
            if (fallback == null)
                fallback = tile;
        }

        return fallback;
    }
}

public enum RoomType
{
    Normal,
    Mob,
    Chest,
    Spawn,
    ExitNormal,
    ExitBoss,
    BossMob
}

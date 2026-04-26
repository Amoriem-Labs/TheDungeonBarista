using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "TDB/Dungeon/TileType")]
public class TileType : ScriptableObject
{
    public string id;
    public bool walkable;
    public bool interactable;
    public bool spawnable;

    [Header("Fallback")]
    public TileBase visualTile;

    [Header("Auto Tiling")]
    public bool useRuleSet = false;
    public TileRuleSet ruleSet;
    public TileNeighborMatchMode neighborMatchMode = TileNeighborMatchMode.SameTileType;
    
    [Header("Extras")]
    public GameObject prefab;
    public bool addYSortIfMissing = true;
}

public enum TileNeighborMatchMode
{
    SameTileType,
    AnyNonNull,
    AnyWalkable
}

using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "TDB/Dungeon/TileType")]

public class TileType : ScriptableObject
{
    public string id;
    public bool walkable;
    public bool interactable;
    public bool spawnable;
    public TileBase visualTile;
}
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TDB/Dungeon/Room Library")]
public class RoomLibrary : ScriptableObject
{
    public List<RoomSO> allRooms;
    public List<RoomTypeRequirement> requirements = new List<RoomTypeRequirement>();
}

[System.Serializable]
public class RoomTypeRequirement
{
    public RoomType roomType = RoomType.Normal;
    [Min(0)] public int minCount = 0;
    public int maxCount = -1;
}

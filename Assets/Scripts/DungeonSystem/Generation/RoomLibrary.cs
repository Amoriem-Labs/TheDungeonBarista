using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TDB/Dungeon/Room Library")]
public class RoomLibrary : ScriptableObject
{
    public List<RoomSO> allRooms;
}

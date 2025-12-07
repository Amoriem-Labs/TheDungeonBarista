using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDB.DungeonSystem.Generate
{
    public class RoomChooser
{
    private RoomLibrary _library;

    public RoomChooser(RoomLibrary library)
    {
        _library = library;
    }

    public RoomSO ChooseRoom(RectInt space)
    {
        // Step 1: Filter rooms that fit inside the BSP partition
        List<RoomSO> candidates = new();

        foreach (var room in _library.allRooms)
        {

            if (room.width <= space.width && room.height <= space.height)
                candidates.Add(room);
        }

        if (candidates.Count == 0)
        {
            Debug.LogWarning("No rooms fit in space: " + space);
            return null;
        }

        // Step 2: Random weighted selection (simple random for now)
        return candidates[Random.Range(0, candidates.Count)];
    }
}

}

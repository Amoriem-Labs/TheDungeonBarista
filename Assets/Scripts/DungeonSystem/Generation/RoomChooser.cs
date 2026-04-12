using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDB.DungeonSystem.Generate
{
    public class RoomChooser
    {
        private readonly RoomLibrary _library;
        private readonly Dictionary<RoomType, int> _assignedCounts = new();
        private readonly Dictionary<RoomType, RoomTypeRequirement> _requirements = new();

        public RoomChooser(RoomLibrary library)
        {
            _library = library;
            BeginGeneration();
        }

        public void BeginGeneration()
        {
            _assignedCounts.Clear();
            _requirements.Clear();

            if (_library == null || _library.requirements == null) return;

            foreach (var req in _library.requirements)
            {
                if (req == null) continue;
                _requirements[req.roomType] = req;
            }
        }

        public void LogUnmetRequirements()
        {
            foreach (var kvp in _requirements)
            {
                var req = kvp.Value;
                int assigned = GetAssignedCount(req.roomType);
                if (assigned < req.minCount)
                {
                    Debug.LogWarning($"Room requirement not met: {req.roomType} needs at least {req.minCount}, assigned {assigned}.");
                }
            }
        }

        public RoomSO ChooseRoom(RectInt space)
        {
            List<RoomSO> candidates = new();
            int padding = 2;

            foreach (var room in _library.allRooms)
            {
                if (room.width <= space.width - padding &&
                    room.height <= space.height - padding)
                {
                    candidates.Add(room);
                }
            }

            if (candidates.Count == 0)
            {
                Debug.LogWarning("No rooms fit in space: " + space);
                return null;
            }

            // Prefer rooms that satisfy remaining minimum requirements.
            List<RoomSO> requiredCandidates = FilterByRemainingRequirements(candidates);
            List<RoomSO> selectable = requiredCandidates.Count > 0 ? requiredCandidates : FilterByMaxCaps(candidates);

            if (selectable.Count == 0)
            {
                Debug.LogWarning("No rooms available after applying requirements for space: " + space);
                return candidates[Random.Range(0, candidates.Count)];
            }

            RoomSO chosen = selectable[Random.Range(0, selectable.Count)];
            IncrementAssigned(chosen.roomType);
            return chosen;
        }

        private List<RoomSO> FilterByRemainingRequirements(List<RoomSO> candidates)
        {
            List<RoomSO> result = new();
            foreach (var room in candidates)
            {
                if (!_requirements.TryGetValue(room.roomType, out var req)) continue;
                int assigned = GetAssignedCount(room.roomType);
                if (assigned < req.minCount && !ExceedsMax(room.roomType, assigned + 1))
                {
                    result.Add(room);
                }
            }
            return result;
        }

        private List<RoomSO> FilterByMaxCaps(List<RoomSO> candidates)
        {
            List<RoomSO> result = new();
            foreach (var room in candidates)
            {
                if (!ExceedsMax(room.roomType, GetAssignedCount(room.roomType) + 1))
                {
                    result.Add(room);
                }
            }
            return result;
        }

        private int GetAssignedCount(RoomType type)
        {
            return _assignedCounts.TryGetValue(type, out var count) ? count : 0;
        }

        private void IncrementAssigned(RoomType type)
        {
            if (_assignedCounts.TryGetValue(type, out var count))
                _assignedCounts[type] = count + 1;
            else
                _assignedCounts[type] = 1;
        }

        private bool ExceedsMax(RoomType type, int nextCount)
        {
            if (_requirements.TryGetValue(type, out var req))
            {
                if (req.maxCount >= 0 && nextCount > req.maxCount)
                    return true;
            }
            return false;
        }
    }

}

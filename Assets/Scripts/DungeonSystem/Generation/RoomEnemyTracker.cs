using UnityEngine;

namespace TDB.DungeonSystem
{
    public class RoomEnemyTracker : MonoBehaviour
    {
        private DungeonRoomController _room;

        public void Initialize(DungeonRoomController room)
        {
            _room = room;
        }

        private void OnDestroy()
        {
            _room?.NotifyEnemyDestroyed(this);
        }
    }
}

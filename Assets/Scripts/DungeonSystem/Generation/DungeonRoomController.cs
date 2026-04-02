using System.Collections.Generic;
using TDB.DungeonSystem.Generate;
using UnityEngine;

namespace TDB.DungeonSystem
{
    public class DungeonRoomController : MonoBehaviour
    {
        private RectInt _bounds;
        private RoomSO _template;
        private DungeonRenderer _renderer;

        private GameObject _enemyPrefab;
        private GameObject _chestPrefab;
        private GameObject _trapPrefab;

        private readonly List<Vector2Int> _enemySpawnCells = new List<Vector2Int>();
        private readonly List<Teleporter> _registeredTeleporters = new List<Teleporter>();

        private int _aliveEnemies;
        private bool _enemiesSpawned;

        public bool IsCleared { get; private set; }

        public void Initialize(
            RectInt bounds,
            RoomSO template,
            DungeonRenderer renderer,
            GameObject enemyPrefab,
            GameObject chestPrefab,
            GameObject trapPrefab,
            List<Vector2Int> enemySpawnCells,
            List<Vector2Int> chestCells,
            List<Vector2Int> trapCells)
        {
            _bounds = bounds;
            _template = template;
            _renderer = renderer;
            _enemyPrefab = enemyPrefab;
            _chestPrefab = chestPrefab;
            _trapPrefab = trapPrefab;

            _enemySpawnCells.Clear();
            _enemySpawnCells.AddRange(enemySpawnCells);

            SpawnStaticPrefabs(chestCells, _chestPrefab);
            SpawnStaticPrefabs(trapCells, _trapPrefab);

            if (_enemySpawnCells.Count == 0 || _enemyPrefab == null)
            {
                IsCleared = true;
            }
        }

        public void RegisterTeleporter(Teleporter teleporter)
        {
            if (teleporter == null) return;
            if (_registeredTeleporters.Contains(teleporter)) return;
            _registeredTeleporters.Add(teleporter);
            teleporter.NotifyInteractableUpdated();
        }

        public void HandlePlayerEntered()
        {
            if (IsCleared || _enemiesSpawned) return;
            SpawnEnemies();
        }

        private void SpawnEnemies()
        {
            _enemiesSpawned = true;

            if (_enemyPrefab == null || _enemySpawnCells.Count == 0)
            {
                MarkCleared();
                return;
            }

            foreach (Vector2Int cell in _enemySpawnCells)
            {
                Vector3 worldPos = GetCellCenterWorld(cell);
                GameObject enemy = Instantiate(_enemyPrefab, worldPos, Quaternion.identity, transform);
                _aliveEnemies++;

                RoomEnemyTracker tracker = enemy.GetComponent<RoomEnemyTracker>();
                if (tracker == null)
                    tracker = enemy.AddComponent<RoomEnemyTracker>();

                tracker.Initialize(this);
            }

            if (_aliveEnemies == 0)
            {
                MarkCleared();
            }
        }

        public void NotifyEnemyDestroyed(RoomEnemyTracker tracker)
        {
            if (_aliveEnemies <= 0) return;
            _aliveEnemies--;
            if (_aliveEnemies == 0)
            {
                MarkCleared();
            }
        }

        private void MarkCleared()
        {
            if (IsCleared) return;
            IsCleared = true;
            for (int i = 0; i < _registeredTeleporters.Count; i++)
            {
                _registeredTeleporters[i]?.NotifyInteractableUpdated();
            }
        }

        private void SpawnStaticPrefabs(List<Vector2Int> cells, GameObject prefab)
        {
            if (prefab == null || cells == null) return;
            for (int i = 0; i < cells.Count; i++)
            {
                Vector3 worldPos = GetCellCenterWorld(cells[i]);
                Instantiate(prefab, worldPos, Quaternion.identity, transform);
            }
        }

        private Vector3 GetCellCenterWorld(Vector2Int cell)
        {
            if (_renderer != null)
                return _renderer.GetCellCenterWorld(cell);
            return new Vector3(cell.x, cell.y, 0f);
        }
    }
}

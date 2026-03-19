using System.Collections.Generic;
using TDB.DungeonSystem.BSP;
using TDB.DungeonSystem.Core;
using TDB.DungeonSystem;
using TDB.Utils.Misc;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

namespace TDB.DungeonSystem.Generate
{
    public class DungeonGenerator : MonoBehaviour
    {
        [System.Serializable]
        private class PropTilePrefab
        {
            public TileType tileType;
            public GameObject prefab;
            public bool addYSortIfMissing = true;
        }

        [Header("Dungeon Settings")]
        public int dungeonWidth = 100;
        public int dungeonHeight = 100;
        public int minLeafSize = 20;
        public int maxLeafSize = 40;
        public int minRoomSize = 6;
        public int maxRoomSize = 12;
        public float aspectRatio = 1.25f;

        // BSP Node list
        private List<BSPNode> leaves = new List<BSPNode>();

        // used for Dungeon Drawing
        public GameObject floorPrefab;
        private DungeonGrid dungeonGrid;
        private TileType[,] wallThemeGrid;
        [SerializeField] private TileType floorTileType;
        [SerializeField] private DungeonRenderer dungeonRenderer;

        // hash table of valid floor tiles for collectible gen
        private HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        [SerializeField] private CollectibleGenerator collectibleGenerator;

        [Header("Teleporters")]
        [SerializeField] private TileType teleporterTileType;
        [SerializeField] private Teleporter teleporterPrefab;
        [SerializeField] private string teleporterContainerName = "_Teleporters";
        [SerializeField] private string teleporterSortingLayerName = "";
        [SerializeField] private int teleporterSortingOrder = 10;
        [SerializeField] private float teleporterZOffset = 0f;
        private Transform _teleporterContainer;
        private readonly HashSet<Vector2Int> _teleporterCells = new HashSet<Vector2Int>();

        [Header("Room Spawns")]
        [SerializeField] private TileType enemySpawnTileType;
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private TileType chestTileType;
        [SerializeField] private GameObject chestPrefab;
        [SerializeField] private TileType trapTileType;
        [SerializeField] private GameObject trapPrefab;
        [SerializeField] private List<PropTilePrefab> propPrefabs = new List<PropTilePrefab>();
        [SerializeField] private string roomControllerContainerName = "_RoomControllers";
        private Transform _roomControllerContainer;
        private readonly Dictionary<RectInt, DungeonRoomController> _roomControllers = new Dictionary<RectInt, DungeonRoomController>();
        private readonly Dictionary<TileType, PropTilePrefab> _propPrefabLookup = new Dictionary<TileType, PropTilePrefab>();

        [SerializeField] private RoomLibrary roomLibrary;
        private RoomChooser _roomChooser;
        void Start()
        {
            _roomChooser = new RoomChooser(roomLibrary);
            GenerateDungeon();
        }

        void GenerateDungeon()
        {
            leaves.Clear();
            dungeonGrid = new DungeonGrid(dungeonWidth, dungeonHeight);
            wallThemeGrid = new TileType[dungeonWidth, dungeonHeight];
            ClearTeleporters();
            _teleporterCells.Clear();
            ClearRoomControllers();
            _roomControllers.Clear();
            BuildPropPrefabLookup();
            Debug.Log("Generating Dungeon...");

            BSPNode root = new BSPNode(new RectInt(0, 0, dungeonWidth, dungeonHeight));
            Split(root);
            Debug.Log("Leaves created: " + leaves.Count);

            _roomChooser.BeginGeneration();
            CreateRooms(root);
            _roomChooser.LogUnmetRequirements();
            ConnectRooms(root);
            GenerateWalls();
            dungeonRenderer.Render(dungeonGrid);
            TrySpawnPlayerAtRoomType(RoomType.Spawn);
            collectibleGenerator.SpawnCollectibles(floorPositions);
        }
        

        void Split(BSPNode node)
        {
            if (node.rect.width < maxLeafSize && node.rect.height < maxLeafSize)
            {
                leaves.Add(node);
                return;
            }

            // randomly choose between horizontal and vertical split
            bool splitHorizontally = Random.value > 0.5f;

            if (node.rect.width > node.rect.height && node.rect.width / node.rect.height >= aspectRatio)
            {
                splitHorizontally = false;
            }
            else if (node.rect.height > node.rect.width && node.rect.height / node.rect.width >= aspectRatio)
            {
                splitHorizontally = true;
            }

            int max = (splitHorizontally ? node.rect.height : node.rect.width) - minLeafSize;

            if (max <= minLeafSize)
            {
                leaves.Add(node);
                return;
            }

            int split = Random.Range(minLeafSize, max);

            if (splitHorizontally)
            {
                node.left = new BSPNode(new RectInt(node.rect.x, node.rect.y, node.rect.width, split));
                node.right = new BSPNode(new RectInt(node.rect.x, node.rect.y + split, node.rect.width, node.rect.height - split));
            }
            else
            {
                node.left = new BSPNode(new RectInt(node.rect.x, node.rect.y, split, node.rect.height));
                node.right = new BSPNode(new RectInt(node.rect.x + split, node.rect.y, node.rect.width - split, node.rect.height));
            }

            // Recurse on the left and right nodes
            Split(node.left);
            Split(node.right);
        }

        void CreateRooms(BSPNode node)
        {
            if (node == null)
            {
                return;
            }

            if (node.IsLeaf())
            {
                FillLeaf(node);
            }
            else
            {
                CreateRooms(node.left);
                CreateRooms(node.right);
            }
        }

        private void FillLeaf(BSPNode node)
        {
            if (!node.IsLeaf()) return;

            RoomSO room = _roomChooser.ChooseRoom(node.rect);
            if (room == null) return;

            int offsetX = node.rect.x + (node.rect.width - room.width) / 2;
            int offsetY = node.rect.y + (node.rect.height - room.height) / 2;
            RectInt roomRect = new RectInt(offsetX, offsetY, room.width, room.height);
            node.room = roomRect;
            node.roomTemplate = room;
            
            if (room.tiles == null || room.tiles.Length != room.width * room.height)
            {
                room.tiles = new TileType[room.width * room.height];
                for (int i = 0; i < room.tiles.Length; i++)
                    room.tiles[i] = room.tiles[i];
            }

            List<Vector2Int> enemySpawns = new List<Vector2Int>();
            List<Vector2Int> chestSpawns = new List<Vector2Int>();
            List<Vector2Int> trapSpawns = new List<Vector2Int>();
            List<(Vector3Int cell, TileType tile)> propSpawns = new List<(Vector3Int, TileType)>();

            for (int y = 0; y < room.height; y++)
            {
                for (int x = 0; x < room.width; x++)
                {
                    int index = x + y * room.width;
                    TileType tile = room.tiles[index];

                    if (tile == null) continue;

                    Vector2Int worldPos = new Vector2Int(offsetX + x, offsetY + y);
                    SetFloorTile(worldPos, tile, ResolveWallTile(room));

                    if (tile == enemySpawnTileType)
                        enemySpawns.Add(worldPos);
                    if (tile == chestTileType)
                        chestSpawns.Add(worldPos);
                    if (tile == trapTileType)
                        trapSpawns.Add(worldPos);

                    if (room.decorationTiles != null && index < room.decorationTiles.Length)
                    {
                        TileType decoration = room.decorationTiles[index];
                        if (decoration != null)
                            propSpawns.Add((new Vector3Int(worldPos.x, worldPos.y, 0), decoration));
                    }
                }
            }

            DungeonRoomController roomController = CreateRoomController(node.room.Value, room, enemySpawns, chestSpawns, trapSpawns);
            _roomControllers[node.room.Value] = roomController;
            SpawnProps(propSpawns, roomController.transform);
        }

        void DrawDungeon()
        {
            // traverse the tree and draw rooms for each leaf
            foreach (var leaf in leaves)
            {
                if (leaf.room.HasValue)
                {
                    Debug.Log($"Drawing room at {leaf.room.Value.position} size {leaf.room.Value.size}");
                    var room = leaf.room.Value;
                    for (int x = room.x; x < room.x + room.width; x++)
                    {
                        for (int y = room.y; y < room.y + room.height; y++)
                        {
                            Vector2Int pos = new Vector2Int(x, y);
                            floorPositions.Add(pos);
                            Instantiate(floorPrefab, new Vector3(x,y, 0), Quaternion.identity);
                        }
                    }
                }
            }
        }

        void ConnectRooms(BSPNode node)
        {
            ConnectSubtrees(node);
        }

        private BSPNode ConnectSubtrees(BSPNode node)
        {
            if (node == null) return null;

            if (node.IsLeaf())
            {
                return (node.room.HasValue && node.roomTemplate != null) ? node : null;
            }

            BSPNode leftRepresentative = ConnectSubtrees(node.left);
            BSPNode rightRepresentative = ConnectSubtrees(node.right);

            if (leftRepresentative != null && rightRepresentative != null)
            {
                CreateCorridor(leftRepresentative, rightRepresentative);
            }

            return ChooseRepresentative(leftRepresentative, rightRepresentative);
        }


        private BSPNode ChooseRepresentative(BSPNode leftRepresentative, BSPNode rightRepresentative)
        {
            if (leftRepresentative == null) return rightRepresentative;
            if (rightRepresentative == null) return leftRepresentative;
            return Random.value > 0.5f ? leftRepresentative : rightRepresentative;
        }

        void CreateCorridor(BSPNode nodeA, BSPNode nodeB)
        {
            if (!nodeA.room.HasValue || !nodeB.room.HasValue) return;
            if (nodeA.roomTemplate == null || nodeB.roomTemplate == null) return;

            if (!TryGetClosestDoorPair(nodeA, nodeB, out Vector2Int pointA, out Vector2Int pointB))
                return;

            //Update used walls:
            if (IsPointOnRoomPerimeter(nodeA.room.Value, pointA))
            {
                WallSide sideA = GetWallSide(nodeA.room.Value, pointA);
                nodeA.UsedWalls.Add(sideA);
            }

            if (IsPointOnRoomPerimeter(nodeB.room.Value, pointB))
            {
                WallSide sideB = GetWallSide(nodeB.room.Value, pointB);
                nodeB.UsedWalls.Add(sideB);
            }

            CreateTeleporterPair(pointA, pointB, nodeA, nodeB);
        }

        private WallSide GetWallSide(RectInt room, Vector2Int point) {
            if (point.y == room.yMax - 1) return WallSide.North;
            if (point.y == room.yMin) return WallSide.South;
            if (point.x == room.xMax - 1) return WallSide.East;
            return WallSide.West;
        }

        private bool IsPointOnRoomPerimeter(RectInt room, Vector2Int point)
        {
            if (point.x < room.xMin || point.y < room.yMin || point.x >= room.xMax || point.y >= room.yMax)
                return false;

            bool onLeftOrRightEdge = point.x == room.xMin || point.x == room.xMax - 1;
            bool onTopOrBottomEdge = point.y == room.yMin || point.y == room.yMax - 1;
            return onLeftOrRightEdge || onTopOrBottomEdge;
        }

        private bool TryGetClosestDoorPair(BSPNode nodeA, BSPNode nodeB, out Vector2Int pointA, out Vector2Int pointB)
        {
            pointA = GetRoomCenter(nodeA.room.Value);
            pointB = GetRoomCenter(nodeB.room.Value);

            List<Vector2Int> candidatesA = BuildTeleporterCandidates(nodeA);
            List<Vector2Int> candidatesB = BuildTeleporterCandidates(nodeB);

            int bestDistance = int.MaxValue;
            bool found = false;

            for (int i = 0; i < candidatesA.Count; i++)
            {
                Vector2Int candidateA = candidatesA[i];
                if (!IsTeleporterCellFree(candidateA)) continue;

                for (int j = 0; j < candidatesB.Count; j++)
                {
                    Vector2Int candidateB = candidatesB[j];
                    if (!IsTeleporterCellFree(candidateB)) continue;

                    int distance = Mathf.Abs(candidateA.x - candidateB.x) + Mathf.Abs(candidateA.y - candidateB.y);
                    if (distance < bestDistance)
                    {
                        bestDistance = distance;
                        pointA = candidateA;
                        pointB = candidateB;
                        found = true;
                    }
                }
            }

            return found;
        }

        private List<Vector2Int> BuildTeleporterCandidates(BSPNode node)
        {
            RectInt roomRect = node.room.Value;
            HashSet<Vector2Int> unique = new HashSet<Vector2Int>();

            List<Vector2Int> doors = node.roomTemplate.GetDoorWorldPositions(roomRect);
            for (int i = 0; i < doors.Count; i++)
            {
                Vector2Int door = doors[i];
                if (IsPointOnRoomPerimeter(roomRect, door))
                    unique.Add(door);
            }

            foreach (Vector2Int cell in EnumerateRoomPerimeter(roomRect))
            {
                unique.Add(cell);
            }

            if (unique.Count == 0)
            {
                unique.Add(GetRoomCenter(roomRect));
            }

            return new List<Vector2Int>(unique);
        }

        private IEnumerable<Vector2Int> EnumerateRoomPerimeter(RectInt room)
        {
            int xMin = room.xMin;
            int xMax = room.xMax - 1;
            int yMin = room.yMin;
            int yMax = room.yMax - 1;

            for (int x = xMin; x <= xMax; x++)
            {
                yield return new Vector2Int(x, yMin);
                if (yMax != yMin)
                    yield return new Vector2Int(x, yMax);
            }

            for (int y = yMin + 1; y <= yMax - 1; y++)
            {
                yield return new Vector2Int(xMin, y);
                if (xMax != xMin)
                    yield return new Vector2Int(xMax, y);
            }
        }

        private Vector2Int GetRoomCenter(RectInt room)
        {
            return new Vector2Int(room.x + room.width / 2, room.y + room.height / 2);
        }

        [SerializeField] private TileType wallTileType; // assign in Inspector

        void GenerateWalls()
        {
            for (int x = 1; x < dungeonWidth - 1; x++)
            {
                for (int y = 1; y < dungeonHeight - 1; y++)
                {
                    if (dungeonGrid.GetTile(new Vector2Int(x, y)) != null) continue;

                    if (TryGetWallTileFromNeighbors(x, y, out TileType wallTile))
                        dungeonGrid.SetTile(new Vector2Int(x, y), wallTile ?? wallTileType);
                }
            }
        }

        bool TryGetWallTileFromNeighbors(int x, int y, out TileType wallTile)
        {
            wallTile = null;
            Vector2Int[] dirs =
            {
                Vector2Int.up,
                Vector2Int.down,
                Vector2Int.left,
                Vector2Int.right,
                new Vector2Int(1, 1),
                new Vector2Int(1, -1),
                new Vector2Int(-1, -1),
                new Vector2Int(-1, 1)
            };

            foreach (var d in dirs)
            {
                int nx = x + d.x;
                int ny = y + d.y;
                TileType neighbor = dungeonGrid.GetTile(new Vector2Int(nx, ny));
                if (neighbor != null && neighbor.walkable)
                {
                    TileType themedWall = wallThemeGrid[nx, ny];
                    if (themedWall != null)
                    {
                        wallTile = themedWall;
                        return true;
                    }

                    if (wallTile == null)
                        wallTile = wallTileType;
                }
            }
            return wallTile != null;
        }

        private void SetFloorTile(Vector2Int pos, TileType floorTile, TileType wallTile)
        {
            if (floorTile == null) return;
            dungeonGrid.SetTile(pos, floorTile);
            if (floorTile.walkable && wallThemeGrid != null)
            {
                wallThemeGrid[pos.x, pos.y] = wallTile ?? wallTileType;
                floorPositions.Add(pos);
            }
        }

        private void SetDecorationTile(Vector2Int pos, TileType decorationTile)
        {
            if (decorationTile == null) return;
            dungeonGrid.SetDecoration(pos, decorationTile);
        }

        private void BuildPropPrefabLookup()
        {
            _propPrefabLookup.Clear();
            for (int i = 0; i < propPrefabs.Count; i++)
            {
                PropTilePrefab entry = propPrefabs[i];
                if (entry == null || entry.tileType == null || entry.prefab == null)
                    continue;
                _propPrefabLookup[entry.tileType] = entry;
            }
        }

        private void SpawnProps(List<(Vector3Int cell, TileType tile)> propSpawns, Transform parent)
        {
            if (propSpawns == null || propSpawns.Count == 0) return;
            for (int i = 0; i < propSpawns.Count; i++)
            {
                TileType tile = propSpawns[i].tile;
                if (tile == null) continue;
                if (!_propPrefabLookup.TryGetValue(tile, out PropTilePrefab entry)) continue;
                SpawnPropAtCell(propSpawns[i].cell, entry.prefab, parent, entry.addYSortIfMissing);
            }
        }

        private GameObject SpawnPropAtCell(Vector3Int cellPosition, GameObject prefab, Transform parent, bool addYSortIfMissing)
        {
            if (prefab == null) return null;

            Vector3 worldPos = dungeonRenderer != null
                ? dungeonRenderer.GetCellCenterWorld(cellPosition)
                : new Vector3(cellPosition.x, cellPosition.y, 0f);

            GameObject obj = Instantiate(prefab, worldPos, Quaternion.identity, parent);
            YSortByPosition sorter = obj.GetComponent<YSortByPosition>();
            if (sorter == null && addYSortIfMissing)
                sorter = obj.AddComponent<YSortByPosition>();
            sorter?.SetOnlyAffectTilemap(false);

            return obj;
        }

        private TileType ResolveWallTile(RoomSO room)
        {
            if (room == null) return wallTileType;
            return room.wallTile != null ? room.wallTile : wallTileType;
        }

        private void CreateTeleporterPair(Vector2Int pointA, Vector2Int pointB, BSPNode nodeA, BSPNode nodeB)
        {
            if (!TryReserveTeleporterCells(pointA, pointB))
                return;

            TileType teleporterTile = teleporterTileType != null ? teleporterTileType : floorTileType;
            if (teleporterTile != null)
            {
                SetFloorTile(pointA, teleporterTile, ResolveWallTile(nodeA.roomTemplate));
                SetFloorTile(pointB, teleporterTile, ResolveWallTile(nodeB.roomTemplate));
            }

            Teleporter teleporterA = SpawnTeleporter(pointA);
            Teleporter teleporterB = SpawnTeleporter(pointB);

            if (teleporterA != null && teleporterB != null)
            {
                DungeonRoomController roomA = GetRoomController(nodeA.room.Value);
                DungeonRoomController roomB = GetRoomController(nodeB.room.Value);

                teleporterA.LinkTo(teleporterB);
                teleporterB.LinkTo(teleporterA);

                teleporterA.SetRoomLinks(roomA, roomB);
                teleporterB.SetRoomLinks(roomB, roomA);

                roomA?.RegisterTeleporter(teleporterA);
                roomB?.RegisterTeleporter(teleporterB);
            }
        }

        private Teleporter SpawnTeleporter(Vector2Int cell)
        {
            if (teleporterPrefab == null) return null;

            EnsureTeleporterContainer();
            Vector3 worldPos = dungeonRenderer != null
                ? dungeonRenderer.GetCellCenterWorld(cell)
                : new Vector3(cell.x, cell.y, 0f);
            worldPos.z += teleporterZOffset;

            Teleporter teleporter = Instantiate(teleporterPrefab, worldPos, Quaternion.identity, _teleporterContainer);
            ApplyTeleporterSorting(teleporter);
            return teleporter;
        }

        private void ApplyTeleporterSorting(Teleporter teleporter)
        {
            if (teleporter == null) return;

            SortingGroup group = teleporter.GetComponentInChildren<SortingGroup>();
            if (group != null)
            {
                if (!string.IsNullOrEmpty(teleporterSortingLayerName))
                    group.sortingLayerName = teleporterSortingLayerName;
                group.sortingOrder = teleporterSortingOrder;
            }

            SpriteRenderer[] renderers = teleporter.GetComponentsInChildren<SpriteRenderer>();
            for (int i = 0; i < renderers.Length; i++)
            {
                if (!string.IsNullOrEmpty(teleporterSortingLayerName))
                    renderers[i].sortingLayerName = teleporterSortingLayerName;
                renderers[i].sortingOrder = teleporterSortingOrder;
            }
        }

        private void EnsureTeleporterContainer()
        {
            if (_teleporterContainer != null) return;
            GameObject container = new GameObject(teleporterContainerName);
            container.transform.SetParent(transform, false);
            _teleporterContainer = container.transform;
        }

        private void ClearTeleporters()
        {
            Transform existing = transform.Find(teleporterContainerName);
            if (existing == null) return;

            if (Application.isPlaying)
                Destroy(existing.gameObject);
            else
                DestroyImmediate(existing.gameObject);

            _teleporterContainer = null;
        }

        private DungeonRoomController CreateRoomController(
            RectInt roomRect,
            RoomSO room,
            List<Vector2Int> enemySpawns,
            List<Vector2Int> chestSpawns,
            List<Vector2Int> trapSpawns)
        {
            EnsureRoomControllerContainer();

            GameObject roomObj = new GameObject($"Room_{roomRect.x}_{roomRect.y}");
            roomObj.transform.SetParent(_roomControllerContainer, false);
            DungeonRoomController controller = roomObj.AddComponent<DungeonRoomController>();
            controller.Initialize(
                roomRect,
                room,
                dungeonRenderer,
                enemyPrefab,
                chestPrefab,
                trapPrefab,
                enemySpawns,
                chestSpawns,
                trapSpawns);
            return controller;
        }

        private DungeonRoomController GetRoomController(RectInt roomRect)
        {
            _roomControllers.TryGetValue(roomRect, out DungeonRoomController controller);
            return controller;
        }

        private void EnsureRoomControllerContainer()
        {
            if (_roomControllerContainer != null) return;
            GameObject container = new GameObject(roomControllerContainerName);
            container.transform.SetParent(transform, false);
            _roomControllerContainer = container.transform;
        }

        private void ClearRoomControllers()
        {
            Transform existing = transform.Find(roomControllerContainerName);
            if (existing == null) return;

            if (Application.isPlaying)
                Destroy(existing.gameObject);
            else
                DestroyImmediate(existing.gameObject);

            _roomControllerContainer = null;
        }

        private bool TryReserveTeleporterCells(Vector2Int cellA, Vector2Int cellB)
        {
            if (!IsTeleporterCellFree(cellA) || !IsTeleporterCellFree(cellB))
                return false;

            bool addedA = _teleporterCells.Add(cellA);
            bool addedB = _teleporterCells.Add(cellB);

            if (addedA && addedB)
                return true;

            if (addedA) _teleporterCells.Remove(cellA);
            if (addedB) _teleporterCells.Remove(cellB);
            return false;
        }

        private bool IsTeleporterCellFree(Vector2Int cell)
        {
            if (cell.x < 0 || cell.y < 0 || cell.x >= dungeonWidth || cell.y >= dungeonHeight)
                return false;

            return !_teleporterCells.Contains(cell);
        }

        private void TrySpawnPlayerAtRoomType(RoomType type)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null) return;

            BSPNode spawnNode = FindRoomByType(type);
            if (spawnNode == null)
            {
                spawnNode = FindFirstRoom();
            }

            if (spawnNode == null) return;

            Vector2Int spawnCell = GetRandomWalkableInRoom(spawnNode);
            Vector3 spawnWorld = dungeonRenderer != null
                ? dungeonRenderer.GetCellCenterWorld(spawnCell)
                : new Vector3(spawnCell.x + 0.5f, spawnCell.y + 0.5f, player.transform.position.z);

            spawnWorld.z = player.transform.position.z;
            player.transform.position = spawnWorld;

            DungeonRoomController spawnRoom = GetRoomController(spawnNode.room.Value);
            spawnRoom?.HandlePlayerEntered();
        }

        private BSPNode FindRoomByType(RoomType type)
        {
            foreach (var leaf in leaves)
            {
                if (!leaf.room.HasValue || leaf.roomTemplate == null) continue;
                if (leaf.roomTemplate.roomType == type)
                    return leaf;
            }
            return null;
        }

        private BSPNode FindFirstRoom()
        {
            foreach (var leaf in leaves)
            {
                if (leaf.room.HasValue && leaf.roomTemplate != null)
                    return leaf;
            }
            return null;
        }

        private Vector2Int GetRandomWalkableInRoom(BSPNode node)
        {
            RectInt roomRect = node.room.Value;
            RoomSO room = node.roomTemplate;

            List<Vector2Int> walkables = new List<Vector2Int>();
            for (int y = 0; y < room.height; y++)
            {
                for (int x = 0; x < room.width; x++)
                {
                    int index = x + y * room.width;
                    TileType tile = room.tiles[index];
                    if (tile != null && tile.walkable)
                    {
                        walkables.Add(new Vector2Int(roomRect.x + x, roomRect.y + y));
                    }
                }
            }

            if (walkables.Count == 0)
            {
                return new Vector2Int(roomRect.x + roomRect.width / 2, roomRect.y + roomRect.height / 2);
            }

            return walkables[Random.Range(0, walkables.Count)];
        }
    }
}

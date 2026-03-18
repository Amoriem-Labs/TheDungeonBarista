using System.Collections.Generic;
using TDB.DungeonSystem.BSP;
using TDB.DungeonSystem.Core;
using TDB.DungeonSystem;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TDB.DungeonSystem.Generate
{
    public class DungeonGenerator : MonoBehaviour
    {
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
        private Transform _teleporterContainer;
        private readonly HashSet<Vector2Int> _teleporterCells = new HashSet<Vector2Int>();

        [SerializeField] private RoomLibrary roomLibrary;
        private RoomChooser _roomChooser;

        //[SerializeField] private GridManager gridManager;   // TODO ADD
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
            //DrawDungeon();
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
            
            // RectInt roomRect = new RectInt(offsetX, offsetY, room.width, room.height);
            // node.room = roomRect;
            // node.roomTemplate = room;
            // TODO: Remove after testing, this generates a standard room
            if (room.tiles == null || room.tiles.Length != room.width * room.height)
            {
                room.tiles = new TileType[room.width * room.height];
                for (int i = 0; i < room.tiles.Length; i++)
                    room.tiles[i] = room.tiles[i];
            }

            for (int y = 0; y < room.height; y++)
            {
                for (int x = 0; x < room.width; x++)
                {
                    int index = x + y * room.width;
                    TileType tile = room.tiles[index];

                    if (tile == null) continue;

                    Vector2Int worldPos = new Vector2Int(offsetX + x, offsetY + y);
                    SetFloorTile(worldPos, tile, ResolveWallTile(room));
                }
            }

            node.room = new RectInt(offsetX, offsetY, room.width, room.height);
            node.roomTemplate = room;
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

            CreateTeleporterPair(pointA, pointB, nodeA.roomTemplate, nodeB.roomTemplate);
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

            List<Vector2Int> doorsA = nodeA.roomTemplate.GetDoorWorldPositions(nodeA.room.Value);
            List<Vector2Int> doorsB = nodeB.roomTemplate.GetDoorWorldPositions(nodeB.room.Value);

            bool usedFallbackA = doorsA.Count == 0;
            bool usedFallbackB = doorsB.Count == 0;

            if (usedFallbackA)
                doorsA = new List<Vector2Int> { pointA };
            if (usedFallbackB)
                doorsB = new List<Vector2Int> { pointB };

            int bestDistance = int.MaxValue;
            bool found = false;

            for (int i = 0; i < doorsA.Count; i++)
            {
                Vector2Int candidateA = doorsA[i];
                if (!IsTeleporterCellFree(candidateA)) continue;

                if (!usedFallbackA)
                {
                    WallSide sideA = GetWallSide(nodeA.room.Value, candidateA);
                    if (nodeA.UsedWalls.Count != 4 && nodeA.UsedWalls.Contains(sideA)) continue;
                }

                for (int j = 0; j < doorsB.Count; j++)
                {
                    Vector2Int candidateB = doorsB[j];
                    if (!IsTeleporterCellFree(candidateB)) continue;

                    if (!usedFallbackB)
                    {
                        WallSide sideB = GetWallSide(nodeB.room.Value, candidateB);
                        if (nodeB.UsedWalls.Count != 4 && nodeB.UsedWalls.Contains(sideB)) continue;
                    }

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

        private TileType ResolveWallTile(RoomSO room)
        {
            if (room == null) return wallTileType;
            return room.wallTile != null ? room.wallTile : wallTileType;
        }

        private void CreateTeleporterPair(Vector2Int pointA, Vector2Int pointB, RoomSO roomA, RoomSO roomB)
        {
            if (!TryReserveTeleporterCells(pointA, pointB))
                return;

            TileType teleporterTile = teleporterTileType != null ? teleporterTileType : floorTileType;
            if (teleporterTile != null)
            {
                SetFloorTile(pointA, teleporterTile, ResolveWallTile(roomA));
                SetFloorTile(pointB, teleporterTile, ResolveWallTile(roomB));
            }

            Teleporter teleporterA = SpawnTeleporter(pointA);
            Teleporter teleporterB = SpawnTeleporter(pointB);

            if (teleporterA != null && teleporterB != null)
            {
                teleporterA.LinkTo(teleporterB);
                teleporterB.LinkTo(teleporterA);
            }
        }

        private Teleporter SpawnTeleporter(Vector2Int cell)
        {
            if (teleporterPrefab == null) return null;

            EnsureTeleporterContainer();
            Vector3 worldPos = dungeonRenderer != null
                ? dungeonRenderer.GetCellCenterWorld(cell)
                : new Vector3(cell.x, cell.y, 0f);

            return Instantiate(teleporterPrefab, worldPos, Quaternion.identity, _teleporterContainer);
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

            if (_teleporterCells.Contains(cell))
                return false;

            TileType tile = dungeonGrid.GetTile(cell);
            return tile != null;
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

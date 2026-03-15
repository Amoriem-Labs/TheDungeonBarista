using System.Collections.Generic;
using TDB.DungeonSystem.BSP;
using TDB.DungeonSystem.Core;
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
        [SerializeField] private TileType floorTileType;
        [SerializeField] private DungeonRenderer dungeonRenderer;

        // hash table of valid floor tiles for collectible gen
        private HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        [SerializeField] private CollectibleGenerator collectibleGenerator;


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
                    dungeonGrid.SetTile(worldPos, tile);
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
            WallSide sideA = GetWallSide(nodeA.room.Value, pointA);
            WallSide sideB = GetWallSide(nodeB.room.Value, pointB);
            nodeA.UsedWalls.Add(sideA);
            nodeB.UsedWalls.Add(sideB);


            // Choose L-shaped corridor direction randomly
            if (Random.value > 0.5f)
            {
                CreateHorizontalCorridor(pointA.x, pointB.x, pointA.y);
                CreateVerticalCorridor(pointA.y, pointB.y, pointB.x);
            }
            else
            {
                CreateVerticalCorridor(pointA.y, pointB.y, pointA.x);
                CreateHorizontalCorridor(pointA.x, pointB.x, pointB.y);
            }
        }

        private WallSide GetWallSide(RectInt room, Vector2Int point) {
            if (point.y == room.yMax - 1) return WallSide.North;
            if (point.y == room.yMin) return WallSide.South;
            if (point.x == room.xMax - 1) return WallSide.East;
            return WallSide.West;
        }

        private bool TryGetClosestDoorPair(BSPNode nodeA, BSPNode nodeB, out Vector2Int pointA, out Vector2Int pointB)
        {
            
            pointA = GetRoomCenter(nodeA.room.Value);
            pointB = GetRoomCenter(nodeB.room.Value);

            List<Vector2Int> doorsA = nodeA.roomTemplate.GetDoorWorldPositions(nodeA.room.Value);
            List<Vector2Int> doorsB = nodeB.roomTemplate.GetDoorWorldPositions(nodeB.room.Value);

            if (doorsA.Count == 0 || doorsB.Count == 0)
            {
                return true;
            }

            int bestDistance = int.MaxValue;
            for (int i = 0; i < doorsA.Count; i++)
            {
                //Check if wall has been used already
                Vector2Int worldA = nodeA.room.Value.position + doorsA[i];
                WallSide sideA = GetWallSide(nodeA.room.Value, worldA);
                if(nodeA.UsedWalls.Count != 4 && nodeA.UsedWalls.Contains(sideA)) continue;

                for (int j = 0; j < doorsB.Count; j++)
                {
                    //Check if wall has been used already
                    Vector2Int worldB = nodeB.room.Value.position + doorsB[i];
                    WallSide sideB = GetWallSide(nodeB.room.Value, worldB);
                    if(nodeB.UsedWalls.Count != 4 && nodeB.UsedWalls.Contains(sideB)) continue;
                    
                    int distance = Mathf.Abs(doorsA[i].x - doorsB[j].x) + Mathf.Abs(doorsA[i].y - doorsB[j].y);
                    if (distance < bestDistance)
                    {
                        bestDistance = distance;
                        pointA = doorsA[i];
                        pointB = doorsB[j];
                    }
                }
            }

            return true;
        }

        private Vector2Int GetRoomCenter(RectInt room)
        {
            return new Vector2Int(room.x + room.width / 2, room.y + room.height / 2);
        }

        void CreateHorizontalCorridor(int xStart, int xEnd, int y)
        {
            for (int x = Mathf.Min(xStart, xEnd); x <= Mathf.Max(xStart, xEnd); x++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                floorPositions.Add(pos);
                dungeonGrid.SetTile(new Vector2Int(x, y), floorTileType);
            }
        }

        void CreateVerticalCorridor(int yStart, int yEnd, int x)
        {
            for (int y = Mathf.Min(yStart, yEnd); y <= Mathf.Max(yStart, yEnd); y++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                floorPositions.Add(pos);
                dungeonGrid.SetTile(new Vector2Int(x, y), floorTileType);
            }
        }

        [SerializeField] private TileType wallTileType; // assign in Inspector

        void GenerateWalls()
        {
            for (int x = 1; x < dungeonWidth - 1; x++)
            {
                for (int y = 1; y < dungeonHeight - 1; y++)
                {
                    if (dungeonGrid.GetTile(new Vector2Int(x, y)) != null) continue;

                    if (HasNeighborFloor(x, y))
                        dungeonGrid.SetTile(new Vector2Int(x, y), wallTileType);
                }
            }
        }

        bool HasNeighborFloor(int x, int y)
        {
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
                var t = dungeonGrid.GetTile(new Vector2Int(x + d.x, y + d.y));
                if (t != null && t.walkable)
                    return true;
            }
            return false;
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

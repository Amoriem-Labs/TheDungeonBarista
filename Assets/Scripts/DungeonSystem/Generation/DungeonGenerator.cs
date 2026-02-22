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

            CreateRooms(root);
            ConnectRooms(root);
            GenerateWalls();
            dungeonRenderer.Render(dungeonGrid);
            //DrawDungeon();
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
                            Instantiate(floorPrefab, new Vector3(x, y, 0), Quaternion.identity);
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
                for (int j = 0; j < doorsB.Count; j++)
                {
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
                dungeonGrid.SetTile(new Vector2Int(x, y), floorTileType);
            }
        }

        void CreateVerticalCorridor(int yStart, int yEnd, int x)
        {
            for (int y = Mathf.Min(yStart, yEnd); y <= Mathf.Max(yStart, yEnd); y++)
            {
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
                Vector2Int.right
            };

            foreach (var d in dirs)
            {
                var t = dungeonGrid.GetTile(new Vector2Int(x + d.x, y + d.y));
                if (t != null && t.walkable)
                    return true;
            }
            return false;
        }
    }
}

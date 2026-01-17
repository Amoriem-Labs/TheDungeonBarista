using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TDB.DungeonSystem.BSP;
using TDB.DungeonSystem.Core;
using UnityEngine;
using UnityEngine.Rendering.Universal;
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
                //int roomWidth = Random.Range(minRoomSize, Mathf.Min(maxRoomSize, node.rect.width - 2));
                //int roomHeight = Random.Range(minRoomSize, Mathf.Min(maxRoomSize, node.rect.height - 2));

                //int roomX = node.rect.x + Random.Range(1, node.rect.width - roomWidth - 1);
                //int roomY = node.rect.y + Random.Range(1, node.rect.height - roomHeight - 1);

                //node.room = new RectInt(roomX, roomY, roomWidth, roomHeight);
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

                    Vector2Int worldPos = new(offsetX + x, offsetY + y);
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
            if (node == null) return;

            // If this node has children, connect their rooms
            if (!node.IsLeaf())
            {
                ConnectRooms(node.left);
                ConnectRooms(node.right);

                if (node.left.room.HasValue && node.right.room.HasValue)
                {
                    CreateCorridor(node.left, node.right);
                }
            }
        }

        void CreateCorridor(BSPNode nodeA, BSPNode nodeB)
        {
            // Safety check
            if (!nodeA.room.HasValue || !nodeB.room.HasValue) return;
            if (nodeA.roomTemplate == null || nodeB.roomTemplate == null) return;

            // Get a door position from each room in world coordinates
            Vector2Int pointA = nodeA.roomTemplate.GetRandomDoorWorld(nodeA.room.Value);
            Vector2Int pointB = nodeB.roomTemplate.GetRandomDoorWorld(nodeB.room.Value);

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
                Vector2Int[] dirs = {
            Vector2Int.up, Vector2Int.down,
            Vector2Int.left, Vector2Int.right
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


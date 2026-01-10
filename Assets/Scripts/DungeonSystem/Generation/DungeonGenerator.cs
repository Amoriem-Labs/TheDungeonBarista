using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TDB.DungeonSystem.BSP;
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
            Debug.Log("Generating Dungeon...");
            BSPNode root = new BSPNode(new RectInt(0, 0, dungeonWidth, dungeonHeight));
            Split(root);
            Debug.Log("Leaves created: " + leaves.Count);

            CreateRooms(root);
            ConnectRooms(root);
            DrawDungeon();
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
            if(node == null)
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

            // Center the room inside the BSP rect
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

                    if (tile != null)
                    {
                        Vector2Int worldPos = new Vector2Int(offsetX + x, offsetY + y);
                        // TODO ADD LATER
                        //gridManager.SetTile(worldPos, tile);

                        // add floor tile to floor tile map
                        Vector2Int pos = new Vector2Int(x, y);
                        floorPositions.Add(pos);
                        Instantiate(floorPrefab, new Vector3(worldPos.x, worldPos.y, 0), Quaternion.identity);

                    }
                }
            }

            node.room = new RectInt(offsetX, offsetY, room.width, room.height);
        }

        void DrawDungeon()
        {
            // traverse the tree and draw rooms for each leaf 
            foreach(var leaf in leaves)
            {
                if (leaf.room.HasValue)
                {
                    Debug.Log($"Drawing room at {leaf.room.Value.position} size {leaf.room.Value.size}");
                    var room = leaf.room.Value;
                    for (int x  = room.x; x < room.x + room.width; x++)
                    {
                        for (int y = room.y; y < room.y +room.height; y++)
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
            if (node == null) return;

            // If this node has children, connect their rooms
            if (!node.IsLeaf())
            {
                ConnectRooms(node.left);
                ConnectRooms(node.right);

                if (node.left.room.HasValue && node.right.room.HasValue)
                {
                    CreateCorridor(node.left.room.Value, node.right.room.Value);
                }
            }
        }

        void CreateCorridor(RectInt roomA, RectInt roomB)
        {
            Vector2Int pointA = new Vector2Int(
                Random.Range(roomA.x + 1, roomA.xMax - 1),
                Random.Range(roomA.y + 1, roomA.yMax - 1));

            Vector2Int pointB = new Vector2Int(
                Random.Range(roomB.x + 1, roomB.xMax - 1),
                Random.Range(roomB.y + 1, roomB.yMax - 1));

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
                Vector2Int pos = new Vector2Int(x, y);
                floorPositions.Add(pos);
                Instantiate(floorPrefab, new Vector3(x, y, 0), Quaternion.identity);
            }
        }

        void CreateVerticalCorridor(int yStart, int yEnd, int x)
        {
            for (int y = Mathf.Min(yStart, yEnd); y <= Mathf.Max(yStart, yEnd); y++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                floorPositions.Add(pos);
                Instantiate(floorPrefab, new Vector3(x, y, 0), Quaternion.identity);
            }
        }


    }
}

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

        void Start()
        {
            GenerateDungeon();
        }

        void GenerateDungeon()
        {
            Debug.Log("Generating Dungeon...");
            BSPNode root = new BSPNode(new RectInt(0, 0, dungeonWidth, dungeonHeight));
            Split(root);
            Debug.Log("Leaves created: " + leaves.Count);

            CreateRooms(root);
            DrawDungeon();
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
                int roomWidth = Random.Range(minRoomSize, Mathf.Min(maxRoomSize, node.rect.width - 2));
                int roomHeight = Random.Range(minRoomSize, Mathf.Min(maxRoomSize, node.rect.height - 2));

                int roomX = node.rect.x + Random.Range(1, node.rect.width - roomWidth - 1);
                int roomY = node.rect.y + Random.Range(1, node.rect.height - roomHeight - 1);

                node.room = new RectInt(roomX, roomY, roomWidth, roomHeight);
            }
            else
            {
                CreateRooms(node.left);
                CreateRooms(node.right);
            }
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
                            Instantiate(floorPrefab, new Vector3(x,y, 0), Quaternion.identity);
                        }
                    }
                }
            }
        }

    }
}

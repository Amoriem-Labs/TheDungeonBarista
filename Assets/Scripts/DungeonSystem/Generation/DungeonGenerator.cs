using System.Collections;
using System.Collections.Generic;
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
        private List<BSPNode> leaves = new List<BSPNode>();
        // Start is called before the first frame update
        void Start()
        {
            GenerateDungeon();
        }

        void GenerateDungeon()
        {
            BSPNode root = new BSPNode(new RectInt(0, 0, dungeonWidth, dungeonHeight));
            Split(root);
            CreateRooms(root);
            DrawDungeon();
        }

        void Split(BSPNode node)
        {
            if(node.rect.width < maxLeafSize && node.rect.height < maxLeafSize)
            {
                leaves.Add(node);
                return;
            }

            // randomly choose between horizontal and vertical split 
            bool splitHorizontally = Random.value > 0.5f;
            
            if(node.rect.width > node.rect.height && node.rect.width / node.rect.height >= aspectRatio)
            {

            }
        }

        void CreateRooms(BSPNode node)
        {

        }

        void DrawDungeon()
        {

        }
    }
}

//=================================================================================
// File: BSPNode.cs
// Author: Danielle Campisi
// Date: October 25, 2025
// Description: Defines a node in a Binary Space Partitioning tree for dungeon generation. 
//================================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDB.DungeonSystem.BSP
{
    public class BSPNode : MonoBehaviour
    {
        // area of the node
        public RectInt rect;
        public BSPNode left;
        public BSPNode right;
        // rectangle in this room
        public RectInt? room;

        // ================================
        // Public Methods
        // ================================
        public BSPNode(RectInt rect)
        {
            this.rect = rect;
        }
        public bool IsLeaf()
        {
            if(right == null && left == null)
            {
                return true;
            }
            return false;
        }


    }
}

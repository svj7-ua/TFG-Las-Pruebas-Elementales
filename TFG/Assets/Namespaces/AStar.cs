

using System.Collections.Generic;
using UnityEngine;

namespace AStar
{
    // Node class for A* algorithm
    class AStarNode
    {
        public Vector2Int position;
        public float g; // cost
        public float h; // heuristic

        public float F => g + h;        // Total cost (F = G + H)

        public AStarNode(Vector2Int pos, float gCost, float hCost)
        {
            position = pos;
            g = gCost;
            h = hCost;
        }
    }

    // Comparer for AStarNode to sort nodes based on their F value
    class AStarNodeComparer : IComparer<AStarNode>
    {
        public int Compare(AStarNode a, AStarNode b)
        {
            int fComp = a.F.CompareTo(b.F);
            if (fComp != 0) return fComp;
            int xComp = a.position.x.CompareTo(b.position.x);
            if (xComp != 0) return xComp;
            return a.position.y.CompareTo(b.position.y);
        }
    }
}
using UnityEngine;

namespace PathfindingAssembly
{
    public class Node
    {
        public bool walkable;
        public Vector3 worldPosition;
        public int gridX;
        public int gridY;

        public int gCost; // Distance from starting node
        public int hCost; // Heuristic distance to the target node
        public Node parent;

        public Node(bool _walkable, Vector3 _worldPosition, int _gridX, int _gridY)
        {
            walkable = _walkable;
            worldPosition = _worldPosition;
            gridX = _gridX;
            gridY = _gridY;
        }

        public int fCost
        {
            get { return gCost + hCost; }
        }
    }
}
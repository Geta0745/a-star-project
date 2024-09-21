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

        int GetDistance(Node nodeA, Node nodeB)
        {
            int distX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
            int distY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

            int baseCost = (distX > distY) ? 14 * distY + 10 * (distX - distY) : 14 * distX + 10 * (distY - distX);

            // Modify the cost based on obstacle proximity
            if (!nodeB.walkable)
            {
                baseCost += 1000; // High penalty for moving into an obstacle
            }

            return baseCost;
        }

        public int fCost
        {
            get { return gCost + hCost; }
        }
    }
}
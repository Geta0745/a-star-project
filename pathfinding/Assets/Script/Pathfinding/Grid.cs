using System.Collections.Generic;
using UnityEngine;

namespace PathfindingAssembly
{
    public class Grid : MonoBehaviour
    {
        public bool displayGridGizmos;
        public LayerMask unwalkableMask;
        public Vector2 gridWorldSize;
        public float nodeRadius;

        public Node[,] grid;
        public List<Node> path; // Store the path for visualization
        float nodeDiameter;
        public int gridSizeX, gridSizeY;

        void Start()
        {
            nodeDiameter = nodeRadius * 2;
            gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
            gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
            CreateGrid();
        }

        void CreateGrid()
        {
            grid = new Node[gridSizeX, gridSizeY];
            Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

            for (int x = 0; x < gridSizeX; x++)
            {
                for (int y = 0; y < gridSizeY; y++)
                {
                    Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                    bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));
                    grid[x, y] = new Node(walkable, worldPoint, x, y);
                }
            }
        }

        public Node NodeFromWorldPoint(Vector3 worldPosition)
        {
            float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
            float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;
            percentX = Mathf.Clamp01(percentX);
            percentY = Mathf.Clamp01(percentY);

            int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
            int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
            return grid[x, y];
        }

        public List<Node> GetNeighbours(Node node)
        {
            List<Node> neighbours = new List<Node>();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                        continue;

                    int checkX = node.gridX + x;
                    int checkY = node.gridY + y;

                    if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                    {
                        neighbours.Add(grid[checkX, checkY]);
                    }
                }
            }

            return neighbours;
        }

        public Vector3 NodeToWorldPoint(Node node)
        {
            if (node == null)
            {
                throw new System.ArgumentNullException(nameof(node), "Node cannot be null");
            }

            return node.worldPosition;
        }

        // Utility Functions
        public Node FindClosestWalkableNode(Vector3 position)
        {
            Node closestNode = null;
            float closestDistance = float.MaxValue;

            foreach (var node in grid)
            {
                if (node.walkable)
                {
                    float distance = Vector3.Distance(position, node.worldPosition);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestNode = node;
                    }
                }
            }

            return closestNode;
        }

        public float GetPathLength(List<Node> path)
        {
            float totalLength = 0f;
            for (int i = 0; i < path.Count - 1; i++)
            {
                totalLength += Vector3.Distance(path[i].worldPosition, path[i + 1].worldPosition);
            }
            return totalLength;
        }

        public bool IsTargetReachable(Vector3 targetPosition)
        {
            // Get the start and target nodes from the grid based on world positions
            Node startNode = NodeFromWorldPoint(transform.position);
            Node targetNode = NodeFromWorldPoint(targetPosition);

            // Check if both the start and target nodes are walkable
            if (startNode.walkable && targetNode.walkable)
            {
                // Implement a simple pathfinding check like Breadth-First Search (BFS) or A*
                List<Node> openSet = new List<Node>();
                HashSet<Node> closedSet = new HashSet<Node>();

                openSet.Add(startNode);

                while (openSet.Count > 0)
                {
                    Node currentNode = openSet[0];

                    // If we reached the target node, it's reachable
                    if (currentNode == targetNode)
                    {
                        return true;
                    }

                    openSet.Remove(currentNode);
                    closedSet.Add(currentNode);

                    foreach (Node neighbour in GetNeighbours(currentNode))
                    {
                        if (!neighbour.walkable || closedSet.Contains(neighbour))
                        {
                            continue;
                        }

                        if (!openSet.Contains(neighbour))
                        {
                            openSet.Add(neighbour);
                        }
                    }
                }
            }

            // If we exhaust the open set without reaching the target, it's not reachable
            return false;
        }

        public void SetNodeWalkability(Vector2 gridPosition, bool walkable)
        {
            int x = Mathf.RoundToInt(gridPosition.x);
            int y = Mathf.RoundToInt(gridPosition.y);

            if (x >= 0 && x < gridSizeX && y >= 0 && y < gridSizeY)
            {
                grid[x, y].walkable = walkable;
            }
        }

        public List<Node> GetWalkableNodes()
        {
            List<Node> walkableNodes = new List<Node>();
            foreach (var node in grid)
            {
                if (node.walkable)
                {
                    walkableNodes.Add(node);
                }
            }
            return walkableNodes;
        }

        public void ClearPath()
        {
            path = new List<Node>();
        }

        void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

            if (grid != null && displayGridGizmos)
            {
                foreach (Node n in grid)
                {
                    Gizmos.color = (n.walkable) ? Color.white : Color.red;
                    Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
                }
            }

            // Visualize the path
            if (path != null)
            {
                foreach (Node n in path)
                {
                    Gizmos.color = Color.black;
                    Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
                }
            }
        }
    }
}

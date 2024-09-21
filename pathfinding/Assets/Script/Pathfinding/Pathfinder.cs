using System.Collections.Generic;
using UnityEngine;

namespace PathfindingAssembly
{
    public class Pathfinder : MonoBehaviour
    {
        private Grid grid;

        void Start()
        {
            grid = FindObjectOfType<Grid>(); // Find the grid in the scene
        }

        public Vector3[] CalculatePath(Vector3 seekerPosition, Vector3 targetPosition, Seeker seeker,bool applyFunnel = false)
        {
            // Get start and target nodes from the grid
            Node startNode = grid.NodeFromWorldPoint(seekerPosition);
            Node targetNode = grid.NodeFromWorldPoint(targetPosition);
            // Check if both start and target nodes are valid and walkable
            if (!startNode.walkable || !targetNode.walkable)
            {
                targetNode = grid.FindClosestWalkableNode(targetPosition);
            }

            // Perform pathfinding between start and target nodes
            List<Node> path = FindPath(startNode, targetNode);
            if (applyFunnel && path != null)
            {
                Vector3[] waypoints = FunnelPath(path).ToArray(); // Use the optimized funnel method
                return waypoints;
            }
            // Convert path nodes to world positions if a valid path was found
            if (path != null && path.Count > 0)
            {
                Vector3[] waypoints = new Vector3[path.Count];
                for (int i = 0; i < path.Count; i++)
                {
                    waypoints[i] = path[i].worldPosition; // Convert node positions to world positions
                }
                return waypoints;
            }

            // Return an empty array if no path is found
            return new Vector3[0];
        }

        public List<Vector3> FunnelPath(List<Node> path)
        {
            List<Vector3> funnelPoints = new List<Vector3>();

            if (path == null || path.Count == 0)
                return funnelPoints;

            funnelPoints.Add(path[0].worldPosition); // Start with the first point

            for (int i = 1; i < path.Count - 1; i++)
            {
                Node currentNode = path[i];
                Node nextNode = path[i + 1];

                // Get positions
                Vector3 currentPos = currentNode.worldPosition;
                Vector3 nextPos = nextNode.worldPosition;

                // Calculate the direction to the next node
                Vector3 directionToNext = (nextPos - currentPos).normalized;

                // Check if there is a wall on either side
                Vector3 leftCheck = currentPos - Vector3.Cross(Vector3.up, directionToNext) * 0.5f; // Check left
                Vector3 rightCheck = currentPos + Vector3.Cross(Vector3.up, directionToNext) * 0.5f; // Check right

                Node leftNode = grid.NodeFromWorldPoint(leftCheck);
                Node rightNode = grid.NodeFromWorldPoint(rightCheck);

                // Determine if left or right is more open
                bool leftOpen = leftNode != null && leftNode.walkable;
                bool rightOpen = rightNode != null && rightNode.walkable;

                // Add the adjusted position based on wall proximity
                if (leftOpen && !rightOpen)
                {
                    funnelPoints.Add(leftCheck);
                }
                else if (rightOpen && !leftOpen)
                {
                    funnelPoints.Add(rightCheck);
                }
                else
                {
                    funnelPoints.Add(currentPos); // Default to current position if both sides are open
                }
            }

            // Always add the last node to the funnel
            funnelPoints.Add(path[path.Count - 1].worldPosition);

            return funnelPoints;
        }

        List<Node> FindPath(Node startNode, Node targetNode)
        {
            List<Node> openSet = new List<Node>();
            HashSet<Node> closedSet = new HashSet<Node>();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet[0];
                for (int i = 1; i < openSet.Count; i++)
                {
                    if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                    {
                        currentNode = openSet[i];
                    }
                }

                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    return RetracePath(startNode, targetNode);
                }

                foreach (Node neighbour in grid.GetNeighbours(currentNode))
                {
                    if (!neighbour.walkable || closedSet.Contains(neighbour))
                    {
                        continue;
                    }

                    int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                    if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newMovementCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        neighbour.parent = currentNode;

                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                    }
                }
            }

            return null; // No path found
        }

        List<Node> RetracePath(Node startNode, Node endNode)
        {
            List<Node> path = new List<Node>();
            Node currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.parent;
            }
            path.Reverse();

            return path; // Return the calculated path
        }

        int GetDistance(Node nodeA, Node nodeB)
        {
            int distX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
            int distY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

            return (distX > distY) ? 14 * distY + 10 * (distX - distY) : 14 * distX + 10 * (distY - distX);
        }
    }
}
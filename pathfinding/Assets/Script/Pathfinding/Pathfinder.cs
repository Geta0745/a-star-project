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

        public void CalculatePath(Vector3 seekerPosition, Vector3 targetPosition, Seeker seeker)
        {
            Node startNode = grid.NodeFromWorldPoint(seekerPosition);
            Node targetNode = grid.NodeFromWorldPoint(targetPosition);

            if (startNode != null && targetNode != null)
            {
                List<Node> path = FindPath(startNode, targetNode);
                if (path != null)
                {
                    Vector3[] waypoints = new Vector3[path.Count];
                    for (int i = 0; i < path.Count; i++)
                    {
                        waypoints[i] = path[i].worldPosition; // Convert node positions to world positions
                    }
                    seeker.CalculateDirection(waypoints); // Send the path to the seeker to move
                }
            }
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
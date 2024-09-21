using UnityEngine;

namespace PathfindingAssembly
{
    [RequireComponent(typeof(Collider))]
    public class Obstacle : MonoBehaviour
    {
        public bool isWalkable = false; // Set to true to toggle walkability on

        [SerializeField] private Grid grid;
        [SerializeField] private Collider obstacleCollider;

        private void Start()
        {
            grid = FindObjectOfType<Grid>();
            obstacleCollider = GetComponent<Collider>();
            ToggleWalkability(isWalkable);
        }

        private void OnDestroy()
        {
            // Reset walkability when the obstacle is destroyed
            ToggleWalkability(true);
        }

        public void ToggleWalkability(bool makeWalkable)
        {
            if (grid != null)
            {
                // Loop through all nodes in the grid
                for (int x = 0; x < grid.gridSizeX; x++)
                {
                    for (int y = 0; y < grid.gridSizeY; y++)
                    {
                        Node node = grid.grid[x, y];
                        // Check if the node's position is inside the collider bounds
                        if (IsNodeInsideCollider(node))
                        {
                            node.walkable = makeWalkable;
                        }
                    }
                }
            }
        }

        private bool IsNodeInsideCollider(Node node)
        {
            // Check if the node's position is within the collider bounds
            Vector3 closestPoint = obstacleCollider.ClosestPoint(node.worldPosition);
            return Vector3.Distance(closestPoint, node.worldPosition) < 0.01f;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(obstacleCollider.bounds.center, obstacleCollider.bounds.size);
        }

        // Call this method to toggle the walkability in-game
        public void ToggleObstacle()
        {
            isWalkable = !isWalkable;
            ToggleWalkability(isWalkable);
        }
    }
}

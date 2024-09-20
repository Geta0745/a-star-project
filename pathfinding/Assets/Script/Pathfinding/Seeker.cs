using UnityEngine;
using System.Collections.Generic;

namespace PathfindingAssembly
{
    public class Seeker : MonoBehaviour
    {
        public Transform target; // The target the seeker is trying to reach
        public float updateInterval = 0.2f; // How often to update the pathfinding
        public float stopDistance = 3f; // Distance to stop from the target
        public float collisionRadius = 1f; // Radius for seeker collision detection
        public float avoidRadius = 2f; // Radius to avoid other seekers
        public float avoidWeight = 1f; // Weight of avoidance steering
        private Pathfinder pathfinder;
        private List<Seeker> allSeekers; // List of all other seekers in the scene

        // Direction variables
        public Vector2 movementDirection; // Resulting direction to move towards
        public Vector3[] currentPath;

        protected virtual void Start()
        {
            pathfinder = FindObjectOfType<Pathfinder>(); // Find the Pathfinder in the scene
            allSeekers = new List<Seeker>(FindObjectsOfType<Seeker>()); // Find all seekers in the scene
            InvokeRepeating("RequestPath", 0f, updateInterval); // Request path updates at intervals
        }

        void RequestPath()
        {
            if (pathfinder != null && target != null)
            {
                float distance = Vector3.Distance(transform.position, target.position);
                if (distance > stopDistance)
                {
                    pathfinder.CalculatePath(transform.position, target.position, this); // Pass positions and this seeker
                }
                else
                {
                    movementDirection = Vector2.zero; // Stop moving if within stop distance
                }
            }
            else
            {
                movementDirection = Vector2.zero;
            }
        }

        public virtual void CalculateDirection(Vector3[] path)
        {
            currentPath = path;
            if (path.Length > 0)
            {
                // Calculate movement direction
                Vector3 direction = (path[0] - transform.position).normalized;
                movementDirection = new Vector2(direction.x, direction.z);

                // Apply collision avoidance with other seekers
                Vector2 avoidance = AvoidOtherSeekers();
                movementDirection += avoidance * avoidWeight;

                // Normalize movement direction after applying avoidance
                movementDirection = movementDirection.normalized;
            }
        }

        Vector2 AvoidOtherSeekers()
        {
            Vector2 avoidance = Vector2.zero;

            foreach (var otherSeeker in allSeekers)
            {
                if (otherSeeker != this) // Ignore self
                {
                    float distance = Vector3.Distance(transform.position, otherSeeker.transform.position);
                    if (distance < avoidRadius) // Check if the other seeker is within the avoidance radius
                    {
                        // Calculate a vector pointing away from the other seeker
                        Vector2 away = new Vector2(transform.position.x - otherSeeker.transform.position.x, transform.position.z - otherSeeker.transform.position.z).normalized;
                        avoidance += away / distance; // Weigh avoidance by distance (closer = stronger avoidance)
                    }
                }
            }

            return avoidance;
        }

        private void OnDrawGizmos()
        {
            // Draw a line to the target
            if (target != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, target.position);
            }

            // Draw an arrow indicating movement direction
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, new Vector3(movementDirection.x, 0, movementDirection.y) * 2f); // Length can be adjusted

            // Draw avoidance radius
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, avoidRadius);
        }

        private void OnDrawGizmosSelected()
        {
            // Draw the stop distance when selected
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, stopDistance);
        }
    }
}

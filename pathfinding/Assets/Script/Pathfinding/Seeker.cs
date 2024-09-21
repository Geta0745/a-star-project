using UnityEngine;
using System.Collections.Generic;

namespace PathfindingAssembly
{
    public class Seeker : MonoBehaviour
    {
        public Transform target; // The target the seeker is trying to reach
        public float updateInterval = 0.2f; // How often to update the pathfinding
        public float pickNextWaypointDistance = 1f; // Distance to switch to next waypoint
        public float stopDistance = 3f; // Distance to stop from the target
        public float collisionRadius = 1f; // Radius for seeker collision detection
        public float avoidRadius = 2f; // Radius to avoid other seekers
        public float avoidWeight = 1f; // Weight of avoidance steering

        private Pathfinder pathfinder; // Cached reference to the Pathfinder

        // Direction variables
        public Vector2 movementDirection; // Resulting direction to move towards
        protected List<Vector3> waypoints = new List<Vector3>(); // Use a List for dynamic waypoints
        [SerializeField] protected bool ApplyFunnel = false;

        protected virtual void Start()
        {
            pathfinder = FindObjectOfType<Pathfinder>(); // Cache the Pathfinder reference
            InvokeRepeating(nameof(RequestPath), 0f, updateInterval); // Request path updates at intervals
        }

        void RequestPath()
        {
            if (pathfinder != null && target != null)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);
                if (distanceToTarget > stopDistance)
                {
                    waypoints = new List<Vector3>(pathfinder.CalculatePath(transform.position, target.position, this, ApplyFunnel)); // Pass positions and this seeker
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

        protected virtual void Update()
        {
            CalculateDirection();
        }

        public virtual void CalculateDirection()
        {
            if (waypoints.Count > 0) // Check if waypoints exist
            {
                // Check if we are close enough to the first waypoint to pick the next one
                while (waypoints.Count > 0 && Vector3.Distance(transform.position, waypoints[0]) < pickNextWaypointDistance)
                {
                    waypoints.RemoveAt(0); // Remove the waypoint as we're close enough
                }

                // If we still have waypoints, calculate the direction
                if (waypoints.Count > 0)
                {
                    Vector3 direction = (waypoints[0] - transform.position).normalized;
                    movementDirection = new Vector2(direction.x, direction.z) + AvoidOtherSeekers() * avoidWeight;

                    // Normalize movement direction
                    movementDirection.Normalize();
                }
                else
                {
                    movementDirection = Vector2.zero; // No waypoints left
                }
            }
            else
            {
                movementDirection = Vector2.zero; // No waypoints
            }
        }


        protected Vector2 AvoidOtherSeekers()
        {
            Vector2 avoidance = Vector2.zero;

            // Use a sphere to detect nearby seekers
            Collider[] nearbySeekers = Physics.OverlapSphere(transform.position, avoidRadius, LayerMask.GetMask("SeekerLayer")); // Ensure your seekers are on the "SeekerLayer"

            foreach (var collider in nearbySeekers)
            {
                Seeker otherSeeker = collider.GetComponent<Seeker>();
                if (otherSeeker != null && otherSeeker != this) // Ignore self
                {
                    float distance = Vector3.Distance(transform.position, otherSeeker.transform.position);
                    Vector2 away = (Vector2)(transform.position - otherSeeker.transform.position).normalized;
                    avoidance += away / distance; // Weigh avoidance by distance
                }
            }

            return avoidance;
        }

        private void OnDrawGizmos()
        {
            if (target != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, target.position);
            }

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, avoidRadius);
        }

        protected void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, stopDistance);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, pickNextWaypointDistance);

            if (waypoints.Count > 0)
            {
                Gizmos.color = Color.blue;

                for (int i = 0; i < waypoints.Count; i++)
                {
                    Gizmos.DrawSphere(waypoints[i], 0.2f); // Adjust sphere size as needed

                    if (i > 0)
                    {
                        Gizmos.DrawLine(waypoints[i - 1], waypoints[i]); // Draw lines between waypoints
                    }
                }

                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(transform.position, waypoints[0]); // Draw line to the first waypoint
            }
        }
    }
}

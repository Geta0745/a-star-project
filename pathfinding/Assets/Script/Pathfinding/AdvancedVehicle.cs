using UnityEngine;
using System.Collections.Generic;

namespace PathfindingAssembly
{
    public class AdvancedVehicle : Seeker
    {
        [SerializeField] protected float maxSpeed = 8f;
        [SerializeField] protected float rotationSpeed = 20f;
        [SerializeField] protected float backwardSpeed = 4f;
        [SerializeField] protected float acceleration = .0008f;
        [SerializeField] protected float deceleration = .005f;
        [SerializeField] protected float currentSpeed;
        [SerializeField] protected float detectGroundDistance = 1f;
        protected Rigidbody rb;
        public bool moveable = true;

        protected override void Start()
        {
            base.Start();
            rb = GetComponent<Rigidbody>();
        }

        public override void CalculateDirection()
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
                    float dotProdRear = Vector3.Dot(transform.right, waypoints[0] - transform.position);
                    float dotProdFront = Vector3.Dot(transform.forward, waypoints[0] - transform.position);

                    // Get avoidance vector from AvoidOtherSeekers() and apply it
                    Vector2 avoidance = AvoidOtherSeekers();

                    // Incorporate avoidance into the movement direction
                    Vector2 adjustedMovement = new Vector2(dotProdFront, dotProdRear) + avoidance * avoidWeight;

                    // Normalize movement direction to prevent over-speeding
                    SetMovement(adjustedMovement.normalized);
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

        /*
        movement.x = move forward and backward
        movement.y = turn vehicle
        */
        protected override void Update()
        {
            base.Update();

            bool groundHit = Physics.Raycast(transform.position, -transform.up, detectGroundDistance);
            // Accelerate or decelerate based on input
            if (movementDirection.x > 0 && moveable && groundHit)
            {
                currentSpeed = Mathf.Lerp(currentSpeed, maxSpeed, acceleration * Mathf.Abs(movementDirection.x));
            }
            else if (movementDirection.x == 0 || !moveable || !groundHit)
            {
                currentSpeed = Mathf.Lerp(currentSpeed, 0f, deceleration);
            }
            else if (movementDirection.x < 0 && moveable && groundHit)
            {
                currentSpeed = Mathf.Lerp(currentSpeed, -backwardSpeed, deceleration * Mathf.Abs(movementDirection.x));
            }
        }

        protected virtual void FixedUpdate()
        {
            Vector3 moveDir = transform.forward * currentSpeed * Time.deltaTime;
            Quaternion rotation = Quaternion.Euler(0f, movementDirection.y * rotationSpeed * Time.deltaTime, 0f);
            rb.MovePosition(rb.position + moveDir);

            if (moveable && Physics.Raycast(transform.position, -transform.up, detectGroundDistance))
            {
                rb.MoveRotation(rb.rotation * rotation);
            }
        }

        public void SetMovement(Vector2 movement)
        {
            this.movementDirection = movement;
        }

        public Vector2 GetMovement()
        {
            return movementDirection;
        }

        public float GetCurrentSpeed()
        {
            return currentSpeed;
        }

        public float GetMaxSpeed()
        {
            return maxSpeed;
        }
    }
}

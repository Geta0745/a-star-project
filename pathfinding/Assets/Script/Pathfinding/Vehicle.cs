using UnityEngine;

namespace PathfindingAssembly
{
    [RequireComponent(typeof(Rigidbody))]
    public class Vehicle : Entity
    {
        public float acceleration = 10f; // Acceleration speed
        public float breakForce = 5f;

        private float currentSpeed = 0f; // Current speed of the vehicle

        protected override void Update()
        {
            base.Update();
            // Handle acceleration and deceleration based on movementDirection
            if (movementDirection != Vector2.zero)
            {
                Accelerate();
                Rotate(new Vector3(movementDirection.x, 0, movementDirection.y));
            }
            else
            {
                Decelerate(); // Decelerate when no movement direction
            }

            // Move the vehicle in the forward direction
            Move(transform.forward);
        }

        protected override void Move(Vector3 direction)
        {
            if (currentSpeed > 0)
            {
                // Move the Rigidbody using physics
                rb.MovePosition(rb.position + direction * currentSpeed * Time.deltaTime);
            }
        }

        private void Accelerate()
        {
            // Increase speed based on acceleration
            currentSpeed += acceleration * Time.deltaTime;
            // Clamp to max speed
            currentSpeed = Mathf.Clamp(currentSpeed, 0, maxSpeed);
        }

        private void Decelerate()
        {
            // Gradually decrease speed
            currentSpeed -= breakForce * Time.deltaTime;
            // Ensure speed doesn't go below zero
            currentSpeed = Mathf.Max(currentSpeed, 0);
        }
    }
}

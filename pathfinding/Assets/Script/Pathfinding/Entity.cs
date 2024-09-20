using UnityEngine;

namespace PathfindingAssembly
{
    [RequireComponent(typeof(Rigidbody))]
    public class Entity : Seeker
    {
        protected Rigidbody rb; // Reference to the Rigidbody component
        [SerializeField]
        protected float maxSpeed = 5f; // Speed at which the entity moves
        [SerializeField]
        protected float rotationSpeed = 4f; // Speed at which the entity rotates

        protected override void Start()
        {
            base.Start(); // Call the base class's Start method
            rb = GetComponent<Rigidbody>(); // Get the Rigidbody component
        }

        protected virtual void Update()
        {
            // If there is a movement direction from the seeker
            if (movementDirection != Vector2.zero)
            {
                // Move and rotate based on the movement direction
                Vector3 moveDirection = new Vector3(movementDirection.x, 0, movementDirection.y);
                Move(moveDirection);
                Rotate(moveDirection);
            }
        }

        protected virtual void Move(Vector3 direction)
        {
            // Move the Rigidbody using physics
            rb.MovePosition(rb.position + direction * maxSpeed * Time.deltaTime);
        }

        protected virtual void Rotate(Vector3 direction)
        {
            if (direction != Vector3.zero)
            {
                // Calculate the target rotation
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                // Smoothly rotate towards the target direction
                rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.deltaTime));
            }
        }
    }
}
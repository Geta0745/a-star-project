using System.Collections.Generic;
using UnityEngine;

namespace PathfindingAssembly
{
    public class BlobManager : MonoBehaviour
    {
        public GameObject blobPrefab; // Reference to the blob prefab
        public Transform blobTarget; // Target for the leader
        public int blobCount = 10; // Number of blobs to spawn
        public float leaderStopDistance = 3f; // Distance at which the leader is considered stopped
        public float memberStopDistance = 2f; // Distance between blobs
        public float scatterDistance = 3f; // Distance to scatter around the leader

        [SerializeField] List<BlobMember> blobs = new List<BlobMember>();
        [SerializeField] private BlobMember leader; // The leader blob

        void Start()
        {
            if(blobs.Count == 0)
            {
                SpawnBlobs();
            }
        }

        void SpawnBlobs()
        {
            Vector3 lastSpawnPosition = Vector3.zero; // Starting position for the first blob

            for (int i = 0; i < blobCount; i++)
            {
                // Generate a random offset based on the last blob's position
                Vector3 spawnOffset = new Vector3(
                    Random.Range(-5f, 5f),
                    0.2f,
                    Random.Range(-5f, 5f)
                );

                Vector3 spawnPosition = lastSpawnPosition + spawnOffset; // New position based on last blob's position
                GameObject blob = Instantiate(blobPrefab, transform.position + spawnPosition, Quaternion.identity);
                GameObject guideTargetGameObject = new GameObject("GuideTarget");
                guideTargetGameObject.transform.SetParent(blob.transform);
                guideTargetGameObject.transform.localPosition = Vector3.zero;
                BlobMember member = blob.AddComponent<BlobMember>();
                if (member != null)
                {
                    member.seeker = blob.GetComponent<Seeker>();
                    member.guideTarget = guideTargetGameObject.transform;
                    member.seeker.target = member.guideTarget; // Initially, no target
                    member.seeker.stopDistance = memberStopDistance;
                    blobs.Add(member);
                }

                lastSpawnPosition = spawnPosition; // Update lastSpawnPosition for next blob
            }

            leader = blobs[0]; // Set the first blob as the leader
        }

        void Update()
        {
            // Ensure the leader is following the target
            if (leader != null && blobTarget != null)
            {
                leader.guideTarget.position = blobTarget.position; // Set the leader's target
                leader.seeker.stopDistance = leaderStopDistance;
            }
            else
            {
                leader = blobs[0];
                leader.seeker.stopDistance = leaderStopDistance;
            }

            // Arrange other blobs around the leader in a specific pattern
            ScatterBlobs();
        }

        void ScatterBlobs()
        {
            // Calculate the angle step based on the number of blobs
            float angleStep = 360f / (blobs.Count - 1);
            float radius = scatterDistance; // Distance from the leader

            for (int i = 1; i < blobs.Count; i++)
            {
                if (blobs[i] != null)
                {
                    // Calculate the angle for this blob (i-th blob)
                    float angle = angleStep * i;
                    blobs[i].seeker.stopDistance = memberStopDistance;

                    // Convert the angle to a position around the leader
                    Vector3 scatterPosition = new Vector3(
                        leader.transform.position.x + Mathf.Cos(angle * Mathf.Deg2Rad) * radius,
                        leader.transform.position.y,
                        leader.transform.position.z + Mathf.Sin(angle * Mathf.Deg2Rad) * radius
                    );

                    // Set the guide target for this blob to scatter around the leader
                    blobs[i].guideTarget.position = scatterPosition;
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (leader != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(leader.transform.position, scatterDistance); // Draw a visual representation of the scatter radius
            }
        }
    }
}

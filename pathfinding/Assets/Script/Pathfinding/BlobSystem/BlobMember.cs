using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PathfindingAssembly
{
    [RequireComponent(typeof(Seeker))]
    public class BlobMember : MonoBehaviour
    {
        public Transform guideTarget;
        public Seeker seeker;
        // Start is called before the first frame update
        void awake()
        {
            if(seeker == null)
            {
                seeker = GetComponent<Seeker>();
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawSphere(guideTarget.position, 0.2f);
        }
    }
}

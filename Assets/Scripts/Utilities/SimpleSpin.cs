using UnityEngine;

namespace Utilities
{
    public class SimpleSpin : MonoBehaviour
    {
        public bool reverse;
        [SerializeField]
        private Vector3 spin;

        // Update is called once per frame
        void Update()
        {
            var currentRotation = transform.rotation;

            currentRotation *= Quaternion.Euler(spin * (Time.deltaTime * (reverse ? -1 : 1)));

            transform.rotation = currentRotation;
        }
    }
}

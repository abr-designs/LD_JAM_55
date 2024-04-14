using UnityEngine;

namespace Utilities
{
    public class SimpleSpin : MonoBehaviour
    {
        [SerializeField]
        private Vector3 spin;

        // Update is called once per frame
        void Update()
        {
            var currentRotation = transform.rotation;
        
            currentRotation *= Quaternion.Euler(spin * Time.deltaTime);

            transform.rotation = currentRotation;
        }
    }
}

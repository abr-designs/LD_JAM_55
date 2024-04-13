using System;
using System.Collections;
using UnityEngine;

namespace _PROTOTYPE.Scripts
{
    public class VanPrototype : MonoBehaviour
    {
        [SerializeField]
        private Vector2 startLocation;
        [SerializeField]
        private Vector2 endLocation;

        [SerializeField, Min(0f)]
        private float animationTime;

        [SerializeField]
        private AnimationCurve moveCurve;
        
        [SerializeField]
        private TransformAnimator _transformAnimator;

        private bool _isPlaying;

        public void PlayAnimation(Action onAnimationComplete)
        {
            if (_isPlaying)
                return;
            
            StartCoroutine(AnimateCoroutine(onAnimationComplete));
        }

        private IEnumerator AnimateCoroutine(Action onAnimationComplete)
        {
            _isPlaying = true;
            _transformAnimator.Loop();
            var halfTime = animationTime / 2f;

            transform.position = startLocation;

            for (float t = 0; t < halfTime; t+= Time.deltaTime)
            {
                var dt = t / halfTime;

                transform.position = Vector2.Lerp(startLocation, endLocation, moveCurve.Evaluate(dt));
                
                yield return null;
            }
            
            transform.position = endLocation;
            
            //TODO Determine if we need a pause time here
            
            for (float t = 0; t < halfTime; t+= Time.deltaTime)
            {
                var dt = t / halfTime;

                transform.position = Vector2.Lerp(endLocation, startLocation, moveCurve.Evaluate(dt));
                
                yield return null;
            }
            
            transform.position = startLocation;
            
            _transformAnimator.Stop();
            onAnimationComplete?.Invoke();

            _isPlaying = false;
        }

        //Editor Functions
        //============================================================================================================//

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(startLocation, endLocation);
            
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(startLocation, 0.15f);
            
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(endLocation, 0.15f);
        }
#endif
        //============================================================================================================//
    }
}
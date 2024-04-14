using System;
using System.Collections;
using Actors;
using Data;
using UnityEngine;

namespace _PROTOTYPE.Scripts
{
    public class VanPrototype : MonoBehaviour
    {
        [SerializeField]
        private PawnCollector pawnCollector;
        
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

        private UIManager _uiManager;

        private bool _isPlaying;

        //Unity Functions
        //============================================================================================================//

        private void Start()
        {
            pawnCollector.canCollect = true;
            _uiManager = FindObjectOfType<UIManager>();
        }

        //============================================================================================================//

        public void PlayAnimation(Action onAnimationComplete)
        {
            if (_isPlaying)
                return;
            
            StartCoroutine(AnimateCoroutine(onAnimationComplete));
            
        }

        private IEnumerator AnimateCoroutine(Action onAnimationComplete)
        {
            _isPlaying = true;
            pawnCollector.canCollect = false;
            _transformAnimator.Loop();
            var animationTime = this.animationTime * GlobalMults.DeliveryResetTimeMult;

            StartCoroutine(CountdownCoroutine(animationTime));
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
            pawnCollector.canCollect = true;
            onAnimationComplete?.Invoke();

            _isPlaying = false;
        }

        private IEnumerator CountdownCoroutine(float time)
        {
            const string PREFIX = "Next Order In\n";
            
            for (float t = 0; t < time; t+= Time.deltaTime)
            {
                _uiManager.UpdateCountdownText(time - t, PREFIX);
                
                yield return null;
            }
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
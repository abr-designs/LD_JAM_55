using System;
using System.Collections;
using System.Collections.Generic;
using Actors;
using Data;
using UnityEngine;
using Utilities;
using Random = UnityEngine.Random;

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

        [SerializeField]
        private SimpleSpin[] simpleSpinners;

        [SerializeField]
        private SpriteRenderer cageSpriteRenderer;
        [SerializeField]
        private TransformAnimator cageAnimator;
        private List<GameObject> _toDestroy;

        private UIManager _uiManager;

        private bool _isPlaying;

        //Unity Functions
        //============================================================================================================//

        private void Start()
        {
            pawnCollector.CanCollect = true;
            _uiManager = FindObjectOfType<UIManager>();
            ActivateSpinners(false);
            _toDestroy = new List<GameObject>();
        }

        //============================================================================================================//

        public void AddToCage(PawnActor pawnActor)
        {
            cageAnimator.Play();
            
            var bounds = cageSpriteRenderer.localBounds;

            _toDestroy.Add(pawnActor.gameObject);
            
            pawnActor.transform.SetParent(cageSpriteRenderer.transform, false);
            pawnActor.transform.localPosition = Random.insideUnitCircle * bounds.extents.x;
            pawnActor.transform.localRotation = Quaternion.Euler(0f,0f, Random.value * 360f);
            
            pawnActor.enabled = false;
        }

        private void CleanCage()
        {
            for (int i = 0; i < _toDestroy.Count; i++)
            {
                Destroy(_toDestroy[i]);
            }
            
            _toDestroy.Clear();
        }

        public void PlayAnimation(Action onAnimationComplete)
        {
            if (_isPlaying)
                return;
            
            StartCoroutine(AnimateCoroutine(onAnimationComplete));
        }

        private IEnumerator AnimateCoroutine(Action onAnimationComplete)
        {
            _isPlaying = true;
            pawnCollector.CanCollect = false;
            _transformAnimator.Loop();
            ActivateSpinners(true);
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
            CleanCage();
            
            for (float t = 0; t < halfTime; t+= Time.deltaTime)
            {
                var dt = t / halfTime;

                transform.position = Vector2.Lerp(endLocation, startLocation, moveCurve.Evaluate(dt));
                
                yield return null;
            }
            
            transform.position = startLocation;
            
            ActivateSpinners(false);
            _transformAnimator.Stop();
            pawnCollector.CanCollect = true;
            onAnimationComplete?.Invoke();

            _isPlaying = false;
        }

        private void ActivateSpinners(bool state)
        {
            for (int i = 0; i < simpleSpinners.Length; i++)
            {
                simpleSpinners[i].enabled = state;
            }
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
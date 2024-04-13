using System;
using Enums;
using UnityEngine;

namespace Actors
{
    public class PawnCollector : MonoBehaviour
    {
        public static event Action<COLOR> OnCollectedColor;

        [SerializeField]
        private TransformAnimator transformAnimator;
        // Start is called before the first frame update
        void Start()
        {
        
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            var actor = other.gameObject.GetComponent<PawnActor>();
            if (actor == null)
                return;
        
            OnCollectedColor?.Invoke(actor.ActorColor);
            transformAnimator?.Play();
            Destroy(actor.gameObject);
        }
    }
}

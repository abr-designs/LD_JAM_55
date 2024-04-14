using System;
using _PROTOTYPE.Scripts;
using Enums;
using Managers;
using UnityEngine;

namespace Actors
{
    public class PawnCollector : MonoBehaviour
    {
        public static event Action<COLOR> OnCollectedColor;

        public bool CanCollect
        {
            get => _canCollect;
            set => SetCanCollect(value);
        }

        private bool _canCollect;

        [SerializeField]
        private VanPrototype vanPrototype;

        [SerializeField]
        private Collider2D collider2D;

        //Unity Functions
        //============================================================================================================//

        private void OnEnable()
        {
            GameManager.OnGameLost += OnGameLost;
        }

        // Start is called before the first frame update
        void Start()
        {
        
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (_canCollect == false)
                return;

            if (other.gameObject.CompareTag("Actor") == false)
                return;
            
            var actor = other.gameObject.GetComponent<PawnActor>();
            if (actor == null)
                return;
        
            OnCollectedColor?.Invoke(actor.ActorColor);
            vanPrototype.AddToCage(actor);
            //Destroy(actor.gameObject);
        }

        private void OnDisable()
        {
            GameManager.OnGameLost -= OnGameLost;
        }

        //============================================================================================================//

        private void SetCanCollect(bool state)
        {
            collider2D.enabled = state;
            _canCollect = state;
        }

        //Callbacks
        //============================================================================================================//

        private void OnGameLost()
        {
            SetCanCollect(false);
        }
        
        //============================================================================================================//
    }
}

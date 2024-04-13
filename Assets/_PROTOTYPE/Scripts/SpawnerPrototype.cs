using System;
using System.Collections;
using UnityEngine;
using Utilities.ReadOnly;
using Random = UnityEngine.Random;

namespace _PROTOTYPE.Scripts
{
    public class SpawnerPrototype : MonoBehaviour
    {
        [SerializeField, Min(1)]
        private int spawnCount;
        [SerializeField]
        private COLOR spawnColor;
        
        [SerializeField, ReadOnly]
        private int cost;

        [SerializeField, Min(1)]
        private int clickCost;
        private int _timesLeftToClick;

        [SerializeField]
        private Color32 readyColor;
        [SerializeField]
        private Color32 notReadyColor;

        private SpriteRenderer _spriteRenderer;

        [SerializeField]
        private Vector2 waitTimeRange;

        private Vector3 _originalScale;
        private bool _isReady;

        private static GamePrototype _gamePrototype;

        //Unity Functions
        //============================================================================================================//
        
        private void Start()
        {
            if (_gamePrototype == null)
                _gamePrototype = FindObjectOfType<GamePrototype>();

            _spriteRenderer = GetComponent<SpriteRenderer>();
            _spriteRenderer.color = readyColor;

            _timesLeftToClick = clickCost;
            _originalScale = transform.localScale;
        }

        /*private void Update()
        {
            if (_canSpawn)
                return;

            if (_timer > 0f)
            {
                _timer -= Time.deltaTime;
                return;
            }

            _timer = 0f;
            _canSpawn = true;
            _spriteRenderer.color = readyColor;
        }*/
        
        private void OnMouseDown()
        {
            //Don't want to click while we're resetting
            if (isBusy)
                return;
            
            //TODO Want to scale/wobble the item to feedback clicks
            if (_timesLeftToClick > 0)
            {
                _timesLeftToClick--;

                if (_timesLeftToClick == 0)
                {
                    _isReady = true;
                }

            }
            
            if (_isReady == false)
                return;
            
            _gamePrototype.SpawnActors(spawnColor, spawnCount);
            _isReady = false;
            _timesLeftToClick = clickCost;
            _spriteRenderer.color = notReadyColor;

            var waitTime = Random.Range(waitTimeRange.x, waitTimeRange.y);

            StartCoroutine(ResetCoroutine(waitTime, () =>
            {
                _spriteRenderer.color = readyColor;
            }));
        }

        private bool isBusy;
        private IEnumerator ResetCoroutine(float time, Action onComplete)
        {
            if(isBusy)
                yield break;

            isBusy = true;
            
            transform.localScale = Vector3.zero;
            for (float t = 0; t < time; t+=Time.deltaTime)
            {
                transform.localScale = _originalScale * (t / time);
                
                yield return null;
            }

            transform.localScale = _originalScale;
            isBusy = false;
            
            onComplete?.Invoke();
        }
        
        //============================================================================================================//

        private void SpawnActor()
        {
            _gamePrototype.SpawnActor(COLOR.RED);
        }

        //============================================================================================================//
    }
}
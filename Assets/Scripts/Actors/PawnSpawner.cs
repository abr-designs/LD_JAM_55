﻿using System;
using System.Collections;
using Audio;
using Audio.SoundFX;
using Data;
using Enums;
using Managers;
using UnityEngine;
using Utilities;
using Utilities.ReadOnly;
using Random = UnityEngine.Random;

namespace Actors
{
    public class PawnSpawner : MonoBehaviour
    {
        public bool IsActive => isActive;
        
        [SerializeField]
        private bool isActive;
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

        private static GameManager _gameManager;
        private TransformAnimator _transformAnimator;

        [SerializeField]
        private ParticleSystem particleSystem;

        [SerializeField]
        private PhysicsLauncher launcherData;

        //Unity Functions
        //============================================================================================================//
        
        private void Start()
        {
            if (_gameManager == null)
                _gameManager = FindObjectOfType<GameManager>();

            _transformAnimator = GetComponent<TransformAnimator>();

            _spriteRenderer = GetComponent<SpriteRenderer>();
            _spriteRenderer.color = readyColor;

            _timesLeftToClick = clickCost;
            _originalScale = transform.localScale;

            if (isActive == false)
                SetActive(isActive);
        }
        
        private void OnMouseDown()
        {
            SFX.MINE.PlaySound();
            //Don't want to click while we're resetting
            if (isBusy)
            {
                busyTime += 0.05f * GlobalMults.TickleTimeMult;
                _transformAnimator.Play();
                return;
            }
            
            //TODO Want to scale/wobble the item to feedback clicks
            if (_timesLeftToClick > 0)
            {
                _timesLeftToClick--;

                if (_timesLeftToClick == 0)
                {
                    _isReady = true;
                }
                else
                    _transformAnimator.Play();
            }
            
            if (_isReady == false)
                return;
            
            _gameManager.SpawnActors(spawnColor, spawnCount, launcherData);
            _isReady = false;
            _timesLeftToClick = clickCost;
            _spriteRenderer.color = notReadyColor;
            particleSystem.Stop();
            SFX.EXPLODE.PlaySound();

            var waitTime = Random.Range(waitTimeRange.x, waitTimeRange.y) * GlobalMults.SpawnerRegenTimeMult;

            StartCoroutine(ResetCoroutine(waitTime, () =>
            {
                _spriteRenderer.color = readyColor;
                particleSystem.Play();
                particleSystem.Emit(2);
            }));
        }

        //============================================================================================================//

        public void SetActive(bool state)
        {
            _spriteRenderer.enabled = state;

            if (isActive == false && state)
            {
                _isReady = false;
                _timesLeftToClick = clickCost;
                _spriteRenderer.color = notReadyColor;
                particleSystem.Stop();
                
                var waitTime = Random.Range(waitTimeRange.x, waitTimeRange.y);
                StartCoroutine(ResetCoroutine(waitTime, () =>
                {
                    _spriteRenderer.color = readyColor;
                    particleSystem.Play();
                    particleSystem.Emit(2);
                }));
            }

            if (state == false)
                particleSystem.Stop();
                
            isActive = state;
        }
        
        //============================================================================================================//
        

        private bool isBusy;
        private float busyTime;

        private IEnumerator ResetCoroutine(float time, Action onComplete)
        {
            if (isBusy)
                yield break;

            isBusy = true;

            transform.localScale = Vector3.zero;
            for (busyTime = 0; busyTime < time; busyTime += Time.deltaTime)
            {
                transform.localScale = _originalScale * (busyTime / time);

                yield return null;
            }

            transform.localScale = _originalScale;
            isBusy = false;

            onComplete?.Invoke();
        }

        //============================================================================================================//

#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            launcherData.DrawGizmos();
        }

#endif
    }
}
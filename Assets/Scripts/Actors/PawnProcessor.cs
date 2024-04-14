using System.Collections;
using Enums;
using Managers;
using UnityEngine;
using UnityEngine.Assertions;

namespace Actors
{
    public class PawnProcessor : MonoBehaviour
    {
        public COLOR OutColor => outColor;
        
        [SerializeField]
        private COLOR inColor;
        [SerializeField, Min(1)]
        private int inCost;
        
        [SerializeField]
        private COLOR outColor;
        [SerializeField, Min(1)]
        private int outSpawn;

        [SerializeField, Min(0f)]
        private float processingTime;

        private int _paid;
        private bool processing;

        private static GameManager _gameManager;

        [SerializeField]
        private SpriteRenderer[] paidSprites;

        private Color32 _paidColor;
        private Color32 _unpaidColor;

        [SerializeField]
        private SpriteRenderer gemSpriteRenderer;
        [SerializeField]
        private SpriteRenderer[] portalRenderers;

        [SerializeField]
        private TransformAnimator portalAnimator;

        //Unity Functions
        //============================================================================================================//

        private void Start()
        {
            if (_gameManager == null)
                _gameManager = FindObjectOfType<GameManager>();

            SetPortalColor(inColor);
            gemSpriteRenderer.color = _gameManager.colors[(int)outColor];

            SetupPaidUI();
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (processing)
                return;
            
            if (other.gameObject.CompareTag("Actor") == false)
                return;

            var pawnActor = other.gameObject.GetComponent<PawnActor>();

            if (pawnActor.ActorColor != inColor)
                return;

            _paid++;
            portalAnimator.Play();
            UpdatePaid();
            
            Destroy(pawnActor.gameObject);

            if (_paid == inCost)
            {
                StartCoroutine(ProcessingCoroutine(processingTime));
            }
        }

        //============================================================================================================//

        private void SetupPaidUI()
        {
            Assert.IsTrue(paidSprites.Length >= inCost);

            var color = _gameManager.colors[(int)inColor];
            _paidColor = color;
            
            Color.RGBToHSV(color, out var H, out var S, out var V);
            V *= 0.55f;
            _unpaidColor = Color.HSVToRGB(H, S, V);
            
            for (int i = 0; i < paidSprites.Length; i++)
            {
                paidSprites[i].gameObject.SetActive(i <= inCost - 1);
                paidSprites[i].color = _unpaidColor;;
            }
        }

        private void UpdatePaid()
        {
            for (int i = 0; i < paidSprites.Length; i++)
            {
                paidSprites[i].color = i <= _paid - 1 ? _paidColor : _unpaidColor;
            }
        }

        private void SpawnActor()
        {
            _paid = 0;
            _gameManager.SpawnActor(outColor, portalAnimator.transform.position);
            portalAnimator.Play();
            
            UpdatePaid();
        }

        private IEnumerator ProcessingCoroutine(float time)
        {
            processing = true;
            portalAnimator.Loop();

            var inColor32 = _gameManager.colors[(int)this.inColor];
            var outColor32 = _gameManager.colors[(int)this.outColor];
            
            for (float t = 0; t < time; t+= Time.deltaTime)
            {
                SetPortalColor(Color32.Lerp(inColor32, outColor32, t / time));
                yield return null;
            }

            SpawnActor();
            portalAnimator.Stop();
            processing = false;
            SetPortalColor(inColor);
        }

        private void SetPortalColor(COLOR color)
        {
            var color32 = _gameManager.colors[(int)color];
            SetPortalColor(color32);
        }
        
        private void SetPortalColor(Color32 color)
        {
            for (int i = 0; i < portalRenderers.Length; i++)
            {
                portalRenderers[i].color = color;
            }
        }

        //============================================================================================================//
    }
}
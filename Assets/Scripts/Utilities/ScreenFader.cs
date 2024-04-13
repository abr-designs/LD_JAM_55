using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Utilities
{
    public class ScreenFader : MonoBehaviour
    {
        private static readonly Color32 Black = new Color32(0, 0, 0, 255);
        private static readonly Color32 Clear = new Color32(0, 0, 0, 0);
        
        [SerializeField]
        private Image blackImage;

        //
        //============================================================================================================//

        private static ScreenFader _instance;
        
        private void Awake()
        {
            _instance = this;
        }

        //============================================================================================================//
        public static void ForceSetColorBlack()
        {
            _instance.blackImage.color = Black;
        }
        public static void ForceSetColorClear()
        {
            _instance.blackImage.color = Clear;
        }

        public static Coroutine FadeInOut(float time, Action onFaded, Action onComplete)
        {
            return _instance.StartCoroutine(_instance.FadeInOutCoroutine(time, onFaded, onComplete));
        }
        
        public static Coroutine FadeOut(float time, Action onComplete)
        {
            return _instance.StartCoroutine(_instance.FadeCoroutine(Clear, Black, time, onComplete));
        }
        
        public static Coroutine FadeIn(float time, Action onComplete)
        {
            return _instance.StartCoroutine(_instance.FadeCoroutine(Black, Clear, time, onComplete));
        }
        
        //Instance Coroutines
        //============================================================================================================//

        private IEnumerator FadeInOutCoroutine(float time, Action onFaded, Action onComplete)
        {
            var halfTime = time / 2f;

            yield return StartCoroutine(FadeCoroutine(Clear, Black, halfTime, onFaded));
            
            yield return StartCoroutine(FadeCoroutine(Black, Clear, halfTime, onComplete));
        }

        private IEnumerator FadeCoroutine(Color32 startColor, Color32 endColor, float time, Action onCompleted)
        {
            blackImage.color = startColor;

            for (float t = 0; t < time; t += Time.deltaTime)
            {
                blackImage.color = Color32.Lerp(startColor, endColor, t / time);
                yield return null;
            }
            
            blackImage.color = endColor;
            onCompleted?.Invoke();
        }
        //============================================================================================================//
    }
}
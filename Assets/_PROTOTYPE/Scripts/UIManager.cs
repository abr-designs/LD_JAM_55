using System;
using TMPro;
using UnityEngine;

namespace _PROTOTYPE.Scripts
{
    public class UIManager : MonoBehaviour
    {
        private static int COLOR_COUNT => GamePrototype.COLOR_COUNT;

        //============================================================================================================//

        private GamePrototype _gamePrototype;
        
        [SerializeField, Header("UI")]
        private SpriteRenderer[] _spriteRenderers = new SpriteRenderer[COLOR_COUNT];
        [SerializeField]
        private TextMeshPro[] _textMeshPros = new TextMeshPro[COLOR_COUNT];
        [SerializeField]
        private TransformAnimator[] _transformAnimators = new TransformAnimator[COLOR_COUNT];
        [SerializeField]
        private TextMeshPro countDownText;
        [SerializeField]
        private TextMeshPro currencyText;

        //Unit Functions
        //============================================================================================================//
        private void OnEnable()
        {
            GamePrototype.OnCountdownUpdated += UpdateCountdownText;
            GamePrototype.OnCurrencyChanged += OnCurrencyChanged;
            GamePrototype.OnColorRemainingChanged += UpdateColorsToCollectUI;
            GamePrototype.OnColorRemainingSet += SetupOrderUI;
            GamePrototype.OnOrderCompleted += ResetOrderUI;
        }

        private void Start()
        {
            _gamePrototype = FindObjectOfType<GamePrototype>();
            
            SetupUI();
        }

        private void OnDisable()
        {
            GamePrototype.OnCountdownUpdated -= UpdateCountdownText;
            GamePrototype.OnCurrencyChanged -= OnCurrencyChanged;
            GamePrototype.OnColorRemainingChanged -= UpdateColorsToCollectUI;
            GamePrototype.OnColorRemainingSet -= SetupOrderUI;
            GamePrototype.OnOrderCompleted -= ResetOrderUI;
        }

        //============================================================================================================//

        private void SetupUI()
        {
            countDownText.text = string.Empty;
            currencyText.text = "0";

            for (int i = 0; i < COLOR_COUNT; i++)
            {
                _spriteRenderers[i].gameObject.SetActive(false);
                _textMeshPros[i].gameObject.SetActive(false);
            }
        }

        //============================================================================================================//
        
        private void ResetOrderUI()
        {
            for (int i = 0; i < COLOR_COUNT; i++)
            {
                SetupOrderUI(i, 0);
            }

            countDownText.text = string.Empty;
        }
    
        private void SetupOrderUI(int colorIndex, int count)
        {
            var isVisible = count > 0;
            
            _spriteRenderers[colorIndex].color = _gamePrototype.colors[colorIndex];
            _spriteRenderers[colorIndex].gameObject.SetActive(isVisible);
            _textMeshPros[colorIndex].gameObject.SetActive(isVisible);

            if (isVisible == false)
                return;
            
            _textMeshPros[colorIndex].text = count.ToString();
        }

        private void UpdateColorsToCollectUI(int index, int count)
        {
            _transformAnimators[index].Play();
                
            if (count == 0)
            {
                //TODO This should be a check mark
                _textMeshPros[index].text = "xx";
                return;
            }
        
            _textMeshPros[index].text = count.ToString();
        }

        private void UpdateCountdownText(float secondsRemaining)
        {
            //TODO Make this only update every second
            var timeSpan = TimeSpan.FromSeconds(secondsRemaining);
            var mins = timeSpan.Minutes;
            var sec = timeSpan.Seconds;
            countDownText.text = $" {(mins > 0 ? $"{mins}m " : "")}{sec}s";
        }
        
        private void OnCurrencyChanged(int currencyCount)
        {
            currencyText.text = currencyCount.ToString();
        }
        
        //============================================================================================================//
    }
}
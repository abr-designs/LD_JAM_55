using System;
using Data;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _PROTOTYPE.Scripts
{
    public class UIManager : MonoBehaviour
    {
        private static int COLOR_COUNT => GameManager.COLOR_COUNT;

        //============================================================================================================//

        private GameManager GameManager
        {
            get
            {
                if (_gameManager == null)
                    _gameManager = FindObjectOfType<GameManager>();

                return _gameManager;
            }
        }
        private GameManager _gameManager;
        
        [SerializeField, Header("Game UI")]
        private SpriteRenderer[] _spriteRenderers = new SpriteRenderer[COLOR_COUNT];
        [SerializeField]
        private TextMeshPro[] _textMeshPros = new TextMeshPro[COLOR_COUNT];
        [SerializeField]
        private TransformAnimator[] _transformAnimators = new TransformAnimator[COLOR_COUNT];
        
        [SerializeField, Header("Currency UI")]
        private TextMeshPro currencyText;
        [SerializeField]
        private TransformAnimator currencyTransformAnimator;

        [SerializeField, Header("Lost Game Window")]
        private GameObject loseWindowObject;
        [SerializeField]
        private Button restartGameButton;
        [SerializeField]
        private Button mainMenuButton;
        
        [SerializeField, Header("Countdown Text")]
        private TMP_Text countDownText;
        [SerializeField]
        private TransformAnimator countdownTransformAnimator;

        //Unit Functions
        //============================================================================================================//
        private void OnEnable()
        {
            GameManager.OnCountdownUpdated += UpdateOrderCountdownText;
            GameManager.OnCurrencyChanged += OnCurrencyChanged;
            GameManager.OnColorRemainingChanged += UpdateColorsToCollectUI;
            GameManager.OnColorRemainingSet += SetupOrderUI;
            GameManager.OnOrderCompleted += ResetOrderUI;
            GameManager.OnGameLost += OnGameLost;
            
            CurrencyCollectible.OnPickedUpCurrency += OnPickedUpCurrency;
        }

        private void Awake()
        {
            SetupGameUI();
            SetupButtons();
            SetupWindows();
        }

        private void OnDisable()
        {
            GameManager.OnCountdownUpdated -= UpdateOrderCountdownText;
            GameManager.OnCurrencyChanged -= OnCurrencyChanged;
            GameManager.OnColorRemainingChanged -= UpdateColorsToCollectUI;
            GameManager.OnColorRemainingSet -= SetupOrderUI;
            GameManager.OnOrderCompleted -= ResetOrderUI;
            
            GameManager.OnGameLost -= OnGameLost;
            
            CurrencyCollectible.OnPickedUpCurrency -= OnPickedUpCurrency;
        }

        //Setup UI
        //============================================================================================================//

        private void SetupGameUI()
        {
            countDownText.text = string.Empty;
            currencyText.text = "0";

            for (int i = 0; i < COLOR_COUNT; i++)
            {
                _spriteRenderers[i].gameObject.SetActive(false);
                _textMeshPros[i].gameObject.SetActive(false);
            }
        }

        private void SetupButtons()
        {
            restartGameButton.onClick.AddListener(OnRestartGameButtonPressed);
            mainMenuButton.onClick.AddListener(OnMainMenuButtonPressed);
        }

        private void SetupWindows()
        {
            //FIXME I think that the Upgrade window should live here
            loseWindowObject.gameObject.SetActive(false);
        }

        //Lost Game Window
        //============================================================================================================//
        
        private void OnGameLost()
        {
            loseWindowObject.gameObject.SetActive(true);
        }

        private void OnMainMenuButtonPressed()
        {
            SceneManager.LoadScene(Globals.MENU_SCENE_INDEX);
        }
        private void OnRestartGameButtonPressed()
        {
            SceneManager.LoadScene(Globals.GAME_SCENE_INDEX);
        }

        //Game UI Callbacks
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
            
            _spriteRenderers[colorIndex].color = GameManager.colors[colorIndex];
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

        private void UpdateOrderCountdownText(float secondsRemaining)
        {
            const string PREFIX = "Time to Deliver\n";
            
            UpdateCountdownText(secondsRemaining, PREFIX);
            
            if(secondsRemaining < 10)
                countdownTransformAnimator.Play();
        }
        
        public void UpdateCountdownText(float secondsRemaining, string prefix)
        {
            //TODO Make this only update every second
            var timeSpan = TimeSpan.FromSeconds(secondsRemaining);
            var mins = timeSpan.Minutes;
            var sec = timeSpan.Seconds;
            
            countDownText.text = $"{prefix} {(mins > 0 ? $"{mins}m " : "")}{sec}s";
        }
        
        private void OnCurrencyChanged(int currencyCount)
        {
            currencyText.text = currencyCount.ToString();
        }
        
        private void OnPickedUpCurrency(int _)
        {
            currencyTransformAnimator.Play();
        }
        
        
        //============================================================================================================//

        //============================================================================================================//
    }
}
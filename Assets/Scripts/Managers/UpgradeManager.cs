using System;
using System.Collections.Generic;
using Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
using Random = UnityEngine.Random;

namespace Managers
{
    public class UpgradeManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject windowObject;

        [SerializeField]
        private SpawnersManager SpawnersManager;

        [SerializeField]
        private Button[] buttons;
        [SerializeField]
        private TMP_Text[] buttonsText;

        [SerializeField]
        private int[] levelXpRequired;
        private int _levelIndex;

        //Unity Functions
        //============================================================================================================//

        private void OnEnable()
        {
            GameManager.OnCurrencyChanged += OnCurrencyChanged;
        }

        private void Start()
        {
            SetupAllButtons();
            CloseWindow();
        }

        private void OnDisable()
        {
            GameManager.OnCurrencyChanged -= OnCurrencyChanged;
        }

        //Button Setup
        //============================================================================================================//

        private void SetupAllButtons()
        {
            buttons[0].onClick.AddListener(OnSpawnerButtonPressed);
            buttons[1].onClick.AddListener(OnRegenTimeButtonPressed);
            buttons[2].onClick.AddListener(OnDeliveryResetTimeButtonPressed);
            buttons[3].onClick.AddListener(OnTickleTimeButtonPressed);
            buttons[4].onClick.AddListener(OnRewardsButtonPressed);
            
            
            buttonsText[0].text = GlobalMults.SpawnerText;
            buttonsText[1].text = GlobalMults.SpawnerRegenTimeText;
            buttonsText[2].text = GlobalMults.DeliveryResetTimeText;
            buttonsText[3].text = GlobalMults.TickleTimeText;
            buttonsText[4].text = GlobalMults.RewardAmountText;

            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].gameObject.SetActive(false);
            }
        }

        //Xp Calculator
        //============================================================================================================//
        
        private void OnCurrencyChanged(int newXpValue)
        {
            if (_levelIndex >= levelXpRequired.Length)
                return;

            if (newXpValue < levelXpRequired[_levelIndex + 1])
                return;

            _levelIndex++;

            EnableButtons(3);
            OpenWindow();
        }
        
        //Button Selector
        //============================================================================================================//

        private void OpenWindow()
        {
            windowObject.SetActive(true);
            Time.timeScale = 0f;
            UIMousePointer.SetActive(true);
        }
        
        private void EnableButtons(int count)
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].gameObject.SetActive(false);
            }
            var options = new List<int>(5)
            {
                0, 1, 2, 3, 4
            };

            //Want to be at least level 3 to obtain the extra spawners
            if (SpawnersManager.HasLockedSpawners() == false || _levelIndex < 2)
                options.RemoveAt(0);
            
            //TODO Pick random buttons to show
            for (var i = 0; i < count; i++)
            {
                var optionIndex = Random.Range(0, options.Count);
                var index = options[optionIndex];
                
                buttons[index].gameObject.SetActive(true);
                options.RemoveAt(optionIndex);
            }
        }

        private void CloseWindow()
        {
            UIMousePointer.SetActive(false);
            windowObject.SetActive(false);
            Time.timeScale = 1f;
        }
        
        //Callbacks
        //============================================================================================================//

        private void OnSpawnerButtonPressed()
        {
            SpawnersManager.UnlockNextSpawner();
            CloseWindow();
        }
        
        private void OnRegenTimeButtonPressed()
        {
            GlobalMults.SpawnerRegenTimeMult *= 1f + GlobalMults.SpawnerRegenTimeMultDelta;
            CloseWindow();
        }
        
        private void OnDeliveryResetTimeButtonPressed()
        {
            GlobalMults.DeliveryResetTimeMult *= 1f + GlobalMults.DeliveryResetTimeMultDelta;
            CloseWindow();
        }
        
        private void OnTickleTimeButtonPressed()
        {
            GlobalMults.TickleTimeMult *= 1f + GlobalMults.TickleTimeMultDelta;
            CloseWindow();
        }
        
        private void OnRewardsButtonPressed()
        {
            GlobalMults.RewardAmountMult *= 1f + GlobalMults.RewardAmountMultDelta;
            CloseWindow();
        }
        
        //============================================================================================================//
    }
}

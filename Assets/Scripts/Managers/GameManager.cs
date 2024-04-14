using System;
using System.Collections.Generic;
using _PROTOTYPE.Scripts;
using Actors;
using Enums;
using Orders;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        public static event Action OnOrderCompleted;
        public static event Action OnGameLost;
        public static event Action<float> OnCountdownUpdated;
        public static event Action<int> OnCurrencyChanged;
        //Index, Count
        public static event Action<int, int> OnColorRemainingSet; 
        //Index, Count
        public static event Action<int, int> OnColorRemainingChanged; 
    
        public Vector3 MouseWorldPosition => _mouseWorldPosition;
        private Vector3 _mouseWorldPosition;
        private Camera _camera;

        public static readonly int COLOR_COUNT = Enum.GetNames(typeof(COLOR)).Length;

        //============================================================================================================//

        [FormerlySerializedAs("_actorPrefab")] [SerializeField, Header("Spawning")]
        private PawnActor pawnActorPrefab;
        [SerializeField]
        private Transform actorContainerTransform;

        [FormerlySerializedAs("spawnLocation")] [SerializeField, Header("Spawn Info")]
        private Vector2 spawnLocationCenter;
        [SerializeField, Min(0f)]
        private Vector2 spawnArea;

        [SerializeField]
        private int spawnCount;

        //Orders
        //------------------------------------------------//
        [SerializeField, Header("Orders")]
        private Order[] orders;
        private int _orderIndex;
        [SerializeField]
        private VanPrototype van;

        private bool _countingDownOrderTime;
        private float _orderSecondsRemaining;

        //Processors
        //------------------------------------------------//

        private ProcessorsManager _processorsManager;

        //Currency
        //------------------------------------------------//
       [SerializeField]
        private CurrencyCollectible currencyCollectiblePrefab;
       [SerializeField]
        private Vector2 currencySpawnLocation;
       [SerializeField]
        private Vector2 currencySpawnDirection;
       [FormerlySerializedAs("currencySpawnAngleRange")] [SerializeField, Min(0f)]
        private float currencySpawnAngle;
       [SerializeField]
        private Vector2 currencySpawnForceRange;
        
        private int _currency;

        //Colors
        //------------------------------------------------//
    
        [Header("Colors")]
        public Color32[] colors = new Color32[COLOR_COUNT];
        [SerializeField]
        private int[] colorCount = new int[COLOR_COUNT];

        private readonly int[] _colorsToCollect = new int[COLOR_COUNT];

        //Unity Functions
        //============================================================================================================//

        private void OnEnable()
        {
            PawnCollector.OnCollectedColor += OnCollectedColor;
            CurrencyCollectible.OnPickedUpCurrency += OnPickedUpCurrency;
        }

        // Start is called before the first frame update
        private void Start()
        {
            if (_camera == null)
                _camera = Camera.main;

            SpawnRandomActors(spawnCount);
            SetupOrder(orders[_orderIndex]);
        }

        private float _updateTextTimer;
        // Update is called once per frame
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
                SceneManager.LoadScene(0);
        
            _mouseWorldPosition = _camera.ScreenToWorldPoint(Input.mousePosition);
            _mouseWorldPosition.z = 0;

            if (_countingDownOrderTime)
            {
                //FIXME Make this only update every second
                if (_orderSecondsRemaining > 0f)
                {
                    _orderSecondsRemaining -= Time.deltaTime;

                    _updateTextTimer += Time.deltaTime;

                    if (_updateTextTimer > 1f)
                    {
                        _updateTextTimer = 0f;
                        OnCountdownUpdated?.Invoke(_orderSecondsRemaining);
                    }
                    
                    return;
                }
            
                OnLostGame();
            }
        }
    
        private void OnDisable()
        {
            PawnCollector.OnCollectedColor -= OnCollectedColor;
            CurrencyCollectible.OnPickedUpCurrency -= OnPickedUpCurrency;
        }

        //Game Flow
        //============================================================================================================//

        private void OnLevelComplete()
        {
            _countingDownOrderTime = false;
            OnOrderCompleted?.Invoke();
            
            // - Add Points
            SpawnCurrencyCollectibles(orders[_orderIndex].collectibleDrops);
            // - Do Animation
            // - Countdown to next order?
            // - Setup Next Order
            van.PlayAnimation(() =>
            {
                _orderIndex++;
                SetupOrder(orders[_orderIndex]);
            });
        }

        private void OnLostGame()
        {
            Debug.Log($"Lost on Order[{_orderIndex}]");
            
            _countingDownOrderTime = false;
            Time.timeScale = 0.01f;
            OnGameLost?.Invoke();
        }

        //Spawning Pawns
        //============================================================================================================//

        private void SpawnRandomActors(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var color = (COLOR)Random.Range(0, COLOR_COUNT);
                SpawnActor(color, GetRandomPosition());
            }
        }
    
        public void SpawnActors(COLOR color, int count, Vector2 position)
        {
            for (int i = 0; i < count; i++)
            {
                SpawnActor(color, position);
            }
        }

        public void SpawnActor(COLOR color, Vector2 position)
        {
            var colorIndex = (int)color;
            colorCount[colorIndex]++;

            var temp = Instantiate(pawnActorPrefab, position, Quaternion.identity, actorContainerTransform);
            temp.Init(color, this);
        }

        //Spawn Currency Collectibles
        //============================================================================================================//

        private void SpawnCurrencyCollectibles(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var dir = Quaternion.Euler(0f, 0f, Random.Range(-currencySpawnAngle, currencySpawnAngle)) * currencySpawnDirection.normalized;
                var force = Random.Range(currencySpawnForceRange.x, currencySpawnForceRange.y);

                var instance = Instantiate(currencyCollectiblePrefab, currencySpawnLocation, quaternion.identity);
                instance.Launch(dir * force);
            }
        }

        //Public Methods
        //============================================================================================================//
    
        public Vector2 GetRandomPosition()
        {
            var half = spawnArea / 2f;

            var x = Random.Range(-half.x, half.x);
            var y = Random.Range(-half.y, half.y);
        
            return spawnLocationCenter + new Vector2(x, y);
        }
    
        public int GetSortingOrder(float yPos)
        {
            var halfY = spawnArea.y / 2f;
        
            var maxY = spawnLocationCenter.y - halfY;
            var minY = spawnLocationCenter.y + halfY;

            return Mathf.RoundToInt(Mathf.InverseLerp(minY, maxY, yPos) * 10);
        }

        //Collecting Items
        //============================================================================================================//

        private void SetupOrder(Order order)
        {
            for (int i = 0; i < order.orderDatas.Length; i++)
            {
                var color = order.orderDatas[i].color;
                var orderColorIndex = (int)color;
                var count = order.orderDatas[i].count;

                _colorsToCollect[orderColorIndex] = count;

                //We have a color thats not red, make sure we can do something with it
                if (orderColorIndex > 0)
                {
                    if (_processorsManager == null)
                        _processorsManager = FindObjectOfType<ProcessorsManager>();

                    if (_processorsManager.HasProcessor(color) == false)
                        throw new Exception();
                    
                    _processorsManager.EnableProcessor(color);
                }

                OnColorRemainingSet?.Invoke(orderColorIndex, count);
            }

            _countingDownOrderTime = true;
            _orderSecondsRemaining = order.orderTime;
            OnCountdownUpdated?.Invoke(_orderSecondsRemaining);
        }

        private bool CheckOrderComplete()
        {
            for (int i = 0; i < COLOR_COUNT; i++)
            {
                if (_colorsToCollect[i] > 0)
                    return false;
            }

            return true;
        }

        //Callbacks
        //============================================================================================================//
    
        private void OnCollectedColor(COLOR color)
        {
            var colorIndex = (int)color;

            if (_colorsToCollect[colorIndex] > 0)
            {
                _colorsToCollect[colorIndex]--;
            }
            
            if (CheckOrderComplete())
                OnLevelComplete();
        
            OnColorRemainingChanged?.Invoke(colorIndex, _colorsToCollect[colorIndex]);
        }
        
        private void OnPickedUpCurrency(int toAdd)
        {
            _currency += toAdd;
            OnCurrencyChanged?.Invoke(_currency);
        }

        //Unity Editor
        //============================================================================================================//

        #region Unity Editor

#if UNITY_EDITOR

        [SerializeField, Header("EDITOR ONLY - Order Generation")]
        private OrderGenerationData[] orderGenerationData;

        [ContextMenu("Generate Orders")]
        private void GenerateOrders()
        {
            var orders = new List<Order>();

            for (int i = 0; i < orderGenerationData.Length; i++)
            {
                orders.AddRange(orderGenerationData[i].GenerateOrders());
            }

            this.orders = orders.ToArray();
            EditorUtility.SetDirty(this);
        }
    
        private void OnDrawGizmos()
        {
            Gizmos.color =Color.green;
            Gizmos.DrawWireCube(spawnLocationCenter, spawnArea);

            //Currency Spawns
            //------------------------------------------------//

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(currencySpawnLocation, 0.25f);
            
            Gizmos.color =Color.green;
            Gizmos.DrawLine(currencySpawnLocation, currencySpawnLocation + currencySpawnDirection.normalized);
            
            var dir1 = Quaternion.Euler(0f, 0f, -currencySpawnAngle) * currencySpawnDirection.normalized;
            var dir2 = Quaternion.Euler(0f, 0f, currencySpawnAngle) * currencySpawnDirection.normalized;
            
            Gizmos.color =Color.yellow;
            Gizmos.DrawLine(currencySpawnLocation, currencySpawnLocation + (Vector2)dir1.normalized);
            Gizmos.DrawLine(currencySpawnLocation, currencySpawnLocation + (Vector2)dir2.normalized);
            
            //------------------------------------------------//
        
            if (Application.isPlaying == false)
                return;
        
            Gizmos.color =Color.yellow;
            Gizmos.DrawWireSphere(_mouseWorldPosition, 0.25f);
        }
    
#endif

        #endregion //Unity Editor
    
        //============================================================================================================//
    }
}

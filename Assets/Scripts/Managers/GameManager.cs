using System;
using _PROTOTYPE.Scripts;
using Actors;
using Data;
using Enums;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        public static event Action OnOrderCompleted;
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

        //Currency
        //------------------------------------------------//

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
        }

        // Start is called before the first frame update
        private void Start()
        {
            if (_camera == null)
                _camera = Camera.main;

            SpawnRandomActors(spawnCount);
            SetupOrder(orders[_orderIndex]);
        }

        // Update is called once per frame
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
                SceneManager.LoadScene(0);
        
            _mouseWorldPosition = _camera.ScreenToWorldPoint(Input.mousePosition);
            _mouseWorldPosition.z = 0;

            if (_countingDownOrderTime)
            {
                //TODO Make this only update every second
                if (_orderSecondsRemaining > 0f)
                {
                    _orderSecondsRemaining -= Time.deltaTime;
                    OnCountdownUpdated?.Invoke(_orderSecondsRemaining);
                    return;
                }
            
                //TODO Include a losing condition here
            
            }
        }
    
        private void OnDisable()
        {
            PawnCollector.OnCollectedColor -= OnCollectedColor;
        }

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
            //TODO Make something more interesting to look at, just plopping in the pen is not fun

            var temp = Instantiate(pawnActorPrefab, position, Quaternion.identity, actorContainerTransform);
            temp.Init(color, this);
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
                var orderColorIndex = (int)order.orderDatas[i].color;
                var count = order.orderDatas[i].count;

                _colorsToCollect[orderColorIndex] = count;

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
        
            //TODO Wrap up the order
            if (CheckOrderComplete())
            {
                _countingDownOrderTime = false;
                OnOrderCompleted?.Invoke();
            
                // - Add Points
                //TODO Make this more interesting by having them drop out of the van
                _currency += orders[_orderIndex].reward;
                OnCurrencyChanged?.Invoke(_currency);
                // - Do Animation
                // - Countdown to next order?
                // - Setup Next Order
                van.PlayAnimation(() =>
                {
                    //FIXME Make this progress
                    SetupOrder(orders[0]);
                });

            }
        
            OnColorRemainingChanged?.Invoke(colorIndex, _colorsToCollect[colorIndex]);
        }

        //Unity Editor
        //============================================================================================================//

#if UNITY_EDITOR
    
        private void OnDrawGizmos()
        {
            Gizmos.color =Color.green;
            Gizmos.DrawWireCube(spawnLocationCenter, spawnArea);
        
            if (Application.isPlaying == false)
                return;
        
            Gizmos.color =Color.yellow;
            Gizmos.DrawWireSphere(_mouseWorldPosition, 0.25f);
        }
    
#endif
    
        //============================================================================================================//
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using _PROTOTYPE.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public enum COLOR
{
    RED,
    BLUE,
    GREEN,
    PURPLE
}

public class GamePrototype : MonoBehaviour
{
    public Vector3 MouseWorldPosition => _mouseWorldPosition;
    private Vector3 _mouseWorldPosition;
    private Camera _camera;

    public static readonly int COLOR_COUNT = Enum.GetNames(typeof(COLOR)).Length;

    //============================================================================================================//

    [SerializeField, Header("Spawning")]
    private ActorPrototype _actorPrefab;
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

    //Colors
    //------------------------------------------------//
    
    [Header("Colors")]
    public Color32[] colors = new Color32[COLOR_COUNT];
    [SerializeField]
    private int[] colorCount = new int[COLOR_COUNT];

    private int[] colorsToCollect = new int[COLOR_COUNT];

    //Ui
    //------------------------------------------------//
    [SerializeField, Header("UI")]
    private SpriteRenderer[] _spriteRenderers = new SpriteRenderer[COLOR_COUNT];
    [SerializeField]
    private TextMeshPro[] _textMeshPros = new TextMeshPro[COLOR_COUNT];
    [SerializeField]
    private TransformAnimator[] _transformAnimators = new TransformAnimator[COLOR_COUNT];
    
    //Unity Functions
    //============================================================================================================//

    private void OnEnable()
    {
        CollectorPrototype.OnCollectedColor += OnCollectedColor;
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
    }
    
    private void OnDisable()
    {
        CollectorPrototype.OnCollectedColor -= OnCollectedColor;
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

        var temp = Instantiate(_actorPrefab, position, Quaternion.identity, actorContainerTransform);
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
        ResetOrderUI();
        
        for (int i = 0; i < order.orderDatas.Length; i++)
        {
            var orderColorIndex = (int)order.orderDatas[i].color;
            var count = order.orderDatas[i].count;

            colorsToCollect[orderColorIndex] = count;

            SetupOrderUI(orderColorIndex, count);
        }
    }

    private void ResetOrderUI()
    {
        for (int i = 0; i < COLOR_COUNT; i++)
        {
            SetupOrderUI(i, 0);
        }
    }
    
    private void SetupOrderUI(int colorIndex, int count)
    {
        var isVisible = (count > 0);
            
        _spriteRenderers[colorIndex].color = colors[colorIndex];
        _spriteRenderers[colorIndex].gameObject.SetActive(isVisible);
        _textMeshPros[colorIndex].gameObject.SetActive(isVisible);

        if (isVisible == false)
            return;
            
        _textMeshPros[colorIndex].text = count.ToString();
    }

    private bool CheckOrderComplete()
    {
        for (int i = 0; i < COLOR_COUNT; i++)
        {
            if (colorsToCollect[i] > 0)
                return false;
        }

        return true;
    }

    private void UpdateColorsToCollectUI(int index)
    {
        if (colorsToCollect[index] == 0)
        {
            //TODO This should be a check mark
            _textMeshPros[index].text = "xx";
            return;
        }
        
        _textMeshPros[index].text = colorsToCollect[index].ToString();
    }

    //Callbacks
    //============================================================================================================//
    
    private void OnCollectedColor(COLOR color)
    {
        var colorIndex = (int)color;

        if (colorsToCollect[colorIndex] > 0)
        {
            colorsToCollect[colorIndex]--;
        }
        
        _transformAnimators[colorIndex].Play();

        if (CheckOrderComplete())
        {
            ResetOrderUI();
            //TODO Wrap up the order
            // - Add Points
            // - Do Animation
            van.PlayAnimation(() =>
            {
                //FIXME Make this progress
                SetupOrder(orders[0]);
            });
            // - Countdown to next order?
            // - Setup Next Order
        }
        
        UpdateColorsToCollectUI(colorIndex);
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

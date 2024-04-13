using System;
using System.Collections;
using System.Collections.Generic;
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


    [SerializeField]
    private ActorPrototype _actorPrefab;
    [SerializeField]
    private Transform actorContainerTransform;

    [FormerlySerializedAs("spawnLocation")] [SerializeField, Header("Spawn Info")]
    private Vector2 spawnLocationCenter;
    [SerializeField, Min(0f)]
    private Vector2 spawnArea;

    [SerializeField]
    private int spawnCount;

    public Color32[] colors = new Color32[COLOR_COUNT];
    [SerializeField]
    private int[] colorCount = new int[COLOR_COUNT];

    private int[] colorsToCollect = new int[COLOR_COUNT];

    [SerializeField]
    private SpriteRenderer[] _spriteRenderers = new SpriteRenderer[COLOR_COUNT];
    [SerializeField]
    private TextMeshPro[] _textMeshPros = new TextMeshPro[COLOR_COUNT];
    
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
        SetupOrder();
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
            SpawnActor(color);
        }
    }
    
    public void SpawnActors(COLOR color, int count)
    {
        for (int i = 0; i < count; i++)
        {
            SpawnActor(color);
        }
    }

    public void SpawnActor(COLOR color)
    {
        var colorIndex = (int)color;
        colorCount[colorIndex]++;
        //TODO Make something more interesting to look at, just plopping in the pen is not fun
        var position = GetRandomPosition();

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

    private void SetupOrder()
    {
        for (int i = 0; i < COLOR_COUNT; i++)
        {
            //FIXME Don't make this based on what currently is active
            var count = Random.Range(0, colorCount[i] + 1);
            colorsToCollect[i] = count;

            SetupOrderUI(i, count);
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

        if (CheckOrderComplete())
        {
            //TODO Wrap up the order
            // - Add Points
            // - Do Animation
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

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public enum COLOR
{
    RED,
    BLUE
}

public class GamePrototype : MonoBehaviour
{
    
    public static Vector3 MouseWorldPosition => _mouseWorldPosition;
    private static Vector3 _mouseWorldPosition;
    private static Camera _camera;


    [SerializeField]
    private ActorPrototype _actorPrefab;
    [SerializeField]
    private Transform actorContainerTransform;

    [SerializeField, Header("Spawn Info")]
    private Vector2 spawnLocation;
    [SerializeField, Min(0f)]
    private float spawnRadius;

    [SerializeField]
    private int spawnCount;

    [SerializeField]
    private int[] colorCount = new int[2];

    private int[] colorsToCollect = new int[2];

    [SerializeField]
    private SpriteRenderer[] _spriteRenderers = new SpriteRenderer[2];
    [SerializeField]
    private TextMeshPro[] _textMeshPros = new TextMeshPro[2];
    
    //Unity Functions
    //============================================================================================================//

    private void OnEnable()
    {
        CollectorPrototype.OnCollectedColor += CollectorPrototypeOnOnCollectedColor;
    }

    // Start is called before the first frame update
    private void Start()
    {
        if (_camera == null)
            _camera = Camera.main;

        SpawnActors();
        SetOrder();
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
        CollectorPrototype.OnCollectedColor -= CollectorPrototypeOnOnCollectedColor;
    }

    //============================================================================================================//

    private void SpawnActors()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            var colorIndex = Random.Range(0, 2);
            colorCount[colorIndex]++;
            var position = spawnLocation + (Random.insideUnitCircle * spawnRadius);

            var temp = Instantiate(_actorPrefab, position, Quaternion.identity, actorContainerTransform);
            temp.Init((COLOR)colorIndex, this);
        }
    }

    private void SetOrder()
    {
        for (int i = 0; i < 2; i++)
        {
            var count = Random.Range(0, colorCount[i] + 1);

            var isVisible = (count > 0);

            _spriteRenderers[i].gameObject.SetActive(isVisible);
            _textMeshPros[i].gameObject.SetActive(isVisible);

            if (isVisible == false)
                continue;

            colorsToCollect[i] = count;
            _textMeshPros[i].text = count.ToString();
        }
    }
    
    private void CollectorPrototypeOnOnCollectedColor(COLOR color)
    {
        var colorIndex = (int)color;

        if (colorsToCollect[colorIndex] <= 0)
            return;
        
        colorsToCollect[colorIndex]--;
        UpdateColorsToCollect(colorIndex);
    }

    private void UpdateColorsToCollect(int index)
    {
        if (colorsToCollect[index] == 0)
        {
            //TODO This should be a check mark
            _textMeshPros[index].text = "xx";
        }
        
        _textMeshPros[index].text = colorsToCollect[index].ToString();
    }

    //Public Methods
    //============================================================================================================//
    
    public Vector2 GetRandomPosition()
    {
        return spawnLocation + (Random.insideUnitCircle * spawnRadius);
    }
    
    public int GetSortingOrder(float yPos)
    {
        var maxY = spawnLocation.y - spawnRadius;
        var minY = spawnLocation.y + spawnRadius;

        return Mathf.RoundToInt(Mathf.InverseLerp(minY, maxY, yPos) * 10);
    }

    //Unity Editor
    //============================================================================================================//
    
    private void OnDrawGizmos()
    {
        Gizmos.color =Color.green;
        Gizmos.DrawWireSphere(spawnLocation, spawnRadius);
        
        if (Application.isPlaying == false)
            return;
        
        Gizmos.color =Color.yellow;
        Gizmos.DrawWireSphere(_mouseWorldPosition, 0.25f);
    }
    
    //============================================================================================================//
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ActorPrototype : MonoBehaviour
{
    //============================================================================================================//
    
    [SerializeField]
    private bool holding;
    private HingeJoint2D _hingeJoint2D;
    
    [SerializeField]
    private Rigidbody2D _rigidbody2D;

    [SerializeField]
    private SpriteRenderer _spriteRenderer;

    private Vector2 velocity;
    private Vector2 previousPos;
    private Vector2 currentPos;

    public COLOR ActorColor { get; private set; }

    [Header("Movement")]
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private Vector2 MoveLocation;

    [SerializeField, Min(0f)]
    private float moveMaxWaitTime;

    private float _currentWaitTime;
    private float _targetWaitTime;

    [SerializeField]
    private bool isWaiting;

    private GamePrototype _gamePrototype;
    

    //============================================================================================================//
    // Start is called before the first frame update
    private void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
    }

    private void Update()
    {
        _spriteRenderer.sortingOrder = _gamePrototype.GetSortingOrder(transform.position.y);
        
        if (holding || _rigidbody2D.bodyType != RigidbodyType2D.Kinematic)
            return;

        //------------------------------------------------//
        
        if (isWaiting)
        {
            if (_currentWaitTime < _targetWaitTime)
            {
                _currentWaitTime += Time.deltaTime;
                return;
            }

            _currentWaitTime = 0f;
            MoveLocation = _gamePrototype.GetRandomPosition();

            _spriteRenderer.flipX = (MoveLocation.x - transform.position.x) > 0f;
            
            
            isWaiting = false;
        }

        //------------------------------------------------//
        
        if (Vector2.Distance(transform.position, MoveLocation) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, MoveLocation, moveSpeed * Time.deltaTime);
            return;
        }
        
        isWaiting = true;
        _targetWaitTime = Random.Range(0f, moveMaxWaitTime);
    }

    private void FixedUpdate()
    {
        if (holding == false)
            return;

        _hingeJoint2D.connectedAnchor = _gamePrototype.MouseWorldPosition;
        
        previousPos = currentPos;
        currentPos = _rigidbody2D.position;
        velocity = (currentPos - previousPos) / Time.fixedDeltaTime;
    }

    private void OnMouseDown()
    {
        holding = true;
        //TODO Might want this as a hinge??
        _hingeJoint2D = gameObject.AddComponent<HingeJoint2D>();
        _hingeJoint2D.autoConfigureConnectedAnchor = false;
        _hingeJoint2D.anchor = transform.position - _gamePrototype.MouseWorldPosition;
        
        _rigidbody2D.bodyType = RigidbodyType2D.Dynamic;

        currentPos = previousPos = _rigidbody2D.position;
    }

    private void OnMouseUp()
    {
        if (holding == false)
            return;
        
        Destroy(_hingeJoint2D);
        holding = false;
        
        Debug.Log($"Magnitude: {velocity.magnitude}");
        _rigidbody2D.velocity = velocity;
    }
    //============================================================================================================//

    public void Init(COLOR color, GamePrototype gamePrototype)
    {
        ActorColor = color;
        _gamePrototype = gamePrototype;
        
        MoveLocation = _gamePrototype.GetRandomPosition();
        
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.color = _gamePrototype.colors[(int)ActorColor];
    }


    
    //============================================================================================================//
    
}

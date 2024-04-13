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

    [SerializeField]
    private Color32[] colors = new Color32[2];

    [Header("Movement")]
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private Vector2 MoveLocation;

    private Vector2 moveCircle;
    private float moveCircleRadius;
    

    //============================================================================================================//
    // Start is called before the first frame update
    private void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
    }

    private void Update()
    {
        if (holding || _rigidbody2D.bodyType != RigidbodyType2D.Kinematic)
            return;

        if (Vector2.Distance(transform.position, MoveLocation) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, MoveLocation, moveSpeed * Time.deltaTime);
            return;
        }

        MoveLocation = moveCircle + (Random.insideUnitCircle * moveCircleRadius);

    }

    private void FixedUpdate()
    {
        if (holding == false)
            return;

        _hingeJoint2D.connectedAnchor = GamePrototype.MouseWorldPosition;
        
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
        _hingeJoint2D.anchor = transform.position - GamePrototype.MouseWorldPosition;
        
        //_fixedJoint2D.anchor = mouseWorldPosition;
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

    public void Init(COLOR color, Vector2 moveCenter, float moveRadius)
    {
        ActorColor = color;
        moveCircle = moveCenter;
        moveCircleRadius = moveRadius;
        
        MoveLocation = moveCircle + (Random.insideUnitCircle * moveCircleRadius);
        
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.color = colors[(int)ActorColor];
    }

}

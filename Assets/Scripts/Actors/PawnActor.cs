using System;
using Audio;
using Audio.SoundFX;
using Enums;
using Managers;
using UnityEngine;
using Utilities;
using Random = UnityEngine.Random;

namespace Actors
{
    public class PawnActor : MonoBehaviour
    {
        private enum STATE
        {
            NONE,
            SPAWN,
            IDLE,
            MOVING,
            HOLD,
            THROW
        }
        //============================================================================================================//

        [SerializeField, Min(0f)]
        private float spawnForceMult = 1f;
    
        /*[SerializeField]
    private bool holding;*/
        private HingeJoint2D _hingeJoint2D;
    
        private Rigidbody2D _rigidbody2D;
        private SpriteRenderer _spriteRenderer;
        private TransformAnimator _transformAnimator;

        private PhysicsLauncher _launchData;
        
        private Vector2 _velocity;
        private Vector2 _previousPos;
        private Vector2 _currentPos;

        public COLOR ActorColor { get; private set; }

        [Header("Movement")]
        [SerializeField]
        private float moveSpeed;
        private Vector2 _moveLocation;

        [SerializeField, Min(0f)]
        private float moveMaxWaitTime;

        private float _currentWaitTime;
        private float _targetWaitTime;

        private GameManager _gameManager;
    
        private STATE _currentState;

        //============================================================================================================//
        // Start is called before the first frame update
        private void Start()
        {
        
        }

        private void Update()
        {
            _spriteRenderer.sortingOrder = _gameManager.GetSortingOrder(transform.position.y);

            StateUpdate();
        }

        private void FixedUpdate()
        {
            if (_currentState != STATE.HOLD)
                return;

            _hingeJoint2D.connectedAnchor = _gameManager.MouseWorldPosition;
        
            _previousPos = _currentPos;
            _currentPos = _rigidbody2D.position;
            _velocity = (_currentPos - _previousPos) / Time.fixedDeltaTime;
        }

        private void OnMouseDown()
        {
            SetState(STATE.HOLD);
        }

        private void OnDisable()
        {
            _rigidbody2D.simulated = false;
            _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
        }

        //============================================================================================================//

        public void Init(COLOR color, GameManager gameManager, PhysicsLauncher launcherData)
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _transformAnimator = GetComponent<TransformAnimator>();
        
            ActorColor = color;
            _gameManager = gameManager;
        
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _spriteRenderer.color = _gameManager.colors[(int)ActorColor];

            _launchData = launcherData;
        
            SetState(STATE.SPAWN);
        }
    
        //State Functions
        //============================================================================================================//
    
        private void SetState(STATE newState)
        {
            if (newState == _currentState)
                return;
        
            ExitState(_currentState);
            //TODO Determine if we need to have the Previous State
            _currentState = newState;
            EnterState(newState);
        }

        private void EnterState(STATE newState)
        {
            switch (newState)
            {
                case STATE.NONE:
                    break;
                case STATE.SPAWN:
                    _rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
                    _moveLocation = _gameManager.GetRandomPosition();

                    _rigidbody2D.AddForce(_launchData.GetLaunchVelocity(), ForceMode2D.Impulse);
                    _rigidbody2D.AddTorque(Random.Range(-25,25));
                    break;
                case STATE.IDLE:
                    _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
                    _targetWaitTime = Random.Range(0f, moveMaxWaitTime);
                    
                    break;
                case STATE.MOVING:
                    _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
                    _moveLocation = _gameManager.GetRandomPosition();
                    _spriteRenderer.flipX = (_moveLocation.x - transform.position.x) > 0f;
                
                    _transformAnimator.Stop();
                    transform.localRotation = Quaternion.identity;
                    break;
                case STATE.THROW:
                    _rigidbody2D.velocity = _velocity;
                    _rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
                    break;
                case STATE.HOLD:
                    _transformAnimator.Play();
                    _hingeJoint2D = gameObject.AddComponent<HingeJoint2D>();
                    _hingeJoint2D.autoConfigureConnectedAnchor = false;
                    _hingeJoint2D.anchor = transform.position - _gameManager.MouseWorldPosition;
        
                    _rigidbody2D.bodyType = RigidbodyType2D.Dynamic;

                    _currentPos = _previousPos = _rigidbody2D.position;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        private void StateUpdate()
        {
            switch (_currentState)
            {
                case STATE.NONE:
                    break;
                case STATE.SPAWN:
                    SpawnState();
                    break;
                case STATE.IDLE:
                    IdleState();
                    break;
                case STATE.MOVING:
                    MovingState();
                    break;
                case STATE.THROW:
                    ThrowState();
                    break;
                case STATE.HOLD:
                    HoldState();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    
        private void ExitState(STATE oldState)
        {
            switch (oldState)
            {
                case STATE.NONE:
                    break;
                case STATE.SPAWN:
                    _rigidbody2D.velocity = Vector2.zero;
                    _rigidbody2D.angularVelocity = 0f;
                    _transformAnimator.Play();
                    SFX.HIT.PlaySound();
                    break;
                case STATE.IDLE:
                    _currentWaitTime = 0f;
                    break;
                case STATE.MOVING:
                    break;
                case STATE.THROW:
                    break;
                case STATE.HOLD:
                    Destroy(_hingeJoint2D);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        //============================================================================================================//

        private void SpawnState()
        {
            //TODO Fall waiting to align to a random height within walking area
            //TODO When aligned, change to the idle state

            if (transform.position.y > _moveLocation.y)
                return;
        
            SetState(STATE.IDLE);
        }

        private void IdleState()
        {
            if (_currentWaitTime < _targetWaitTime)
            {
                _currentWaitTime += Time.deltaTime;
                return;
            }
            
            SetState(STATE.MOVING);
        }

        private void MovingState()
        {
            if (Vector2.Distance(transform.position, _moveLocation) > 0.1f)
            {
                transform.position = Vector2.MoveTowards(transform.position, _moveLocation, moveSpeed * Time.deltaTime);
                return;
            }
        
            SetState(STATE.IDLE);
        }

        private void HoldState()
        {
            if (Input.GetKeyUp(KeyCode.Mouse0) == false)
                return;
        
            SetState(STATE.THROW);
        }

        private void ThrowState()
        {
            if (_rigidbody2D.IsSleeping() == false)
                return;
        
            SetState(STATE.IDLE);
        }
    
    

        //============================================================================================================//
    
    }
}

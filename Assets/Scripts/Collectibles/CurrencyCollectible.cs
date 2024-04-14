using System;
using Audio;
using Audio.SoundFX;
using Data;
using Managers;
using UnityEngine;
using UnityEngine.Serialization;

public class CurrencyCollectible : MonoBehaviour
{
    public static event Action<int> OnPickedUpCurrency;
    
    private static GameManager _gameManager;

    private Rigidbody2D _rigidbody2D;

    [SerializeField, Min(0)]
    private int currencyValue;

    [SerializeField, Min(0f)]
    private float pickupRadius;

    [SerializeField, Min(0f)]
    private float flySpeed;
    [SerializeField, Min(0f)]
    private float flySpeedAcceleration;

    [SerializeField]
    private Vector2 targetPos;

    [SerializeField]
    private TransformAnimator transformAnimator;

    [SerializeField]
    private ParticleSystem particleSystem;

    private bool _hasPickedUp;

    //Unity Functions
    //============================================================================================================//

    private void OnEnable()
    {
        GameManager.OnGameLost += OnGameLost;
    }

    // Start is called before the first frame update
    private void Start()
    {
        if (_gameManager == null)
            _gameManager = FindObjectOfType<GameManager>();
        
        if (_rigidbody2D)
            return;
        
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
    }

    // Update is called once per frame
    private void Update()
    {
        transform.right = -_rigidbody2D.velocity.normalized;
        
        if (_hasPickedUp == false)
        {
            var sqrMag = (_gameManager.MouseWorldPosition - transform.position).sqrMagnitude;

            if (sqrMag > pickupRadius * pickupRadius)
                return;
            
            _hasPickedUp = true;
            _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
            transformAnimator.Play();
        }

        if (_hasPickedUp)
        {
            //TODO Fly towards the designated Location
            transform.position = Vector2.MoveTowards(transform.position, targetPos, flySpeed * Time.deltaTime);

            flySpeed += flySpeedAcceleration;
            
            var sqrMag = (targetPos - (Vector2)transform.position).sqrMagnitude;

            if (sqrMag > pickupRadius * pickupRadius)
                return;
            
            OnPickedUpCurrency?.Invoke(Mathf.RoundToInt(currencyValue * GlobalMults.RewardAmountMult));
            particleSystem.transform.SetParent(null, true);
            particleSystem.Stop();
            Destroy(gameObject);
            SFX.COLLECT.PlaySound();
        }
    }
    
    private void OnDisable()
    {
        GameManager.OnGameLost -= OnGameLost;
    }
    
    //============================================================================================================//

    public void Launch(Vector2 impulse)
    {
        if (_gameManager == null)
            _gameManager = FindObjectOfType<GameManager>();
        
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        
        _rigidbody2D.AddForce(impulse, ForceMode2D.Impulse);
    }

    //Callbacks
    //============================================================================================================//
    
    private void OnGameLost()
    {
        particleSystem.transform.SetParent(null, true);
        particleSystem.Stop();
        Destroy(gameObject);
    }

    //============================================================================================================//

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, pickupRadius);

        if (_hasPickedUp == false)
            return;
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(targetPos, pickupRadius);
    }

#endif
}

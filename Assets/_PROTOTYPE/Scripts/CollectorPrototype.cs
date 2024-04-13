using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectorPrototype : MonoBehaviour
{
    public static event Action<COLOR> OnCollectedColor; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        var actor = other.gameObject.GetComponent<ActorPrototype>();
        if (actor == null)
            return;
        
        OnCollectedColor?.Invoke(actor.ActorColor);
        Destroy(actor.gameObject);
    }
}
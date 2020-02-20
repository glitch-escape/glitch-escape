using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldBarrier : MonoBehaviour {
    public enum CollisionType {
        Trigger,
        Collider
    };
    public CollisionType collisionType;
    public Player activePlayer { get; private set; } = null;
    public Collider collider => _collider ?? (_collider = GetComponent<Collider>()) 
                                ?? (Debug.LogError("FieldBarrier.cs: missing collider!"), null));
    private IFieldBarrierEffect[] effects => _effects ?? (_effects = GetComponents<IFieldBarrierEffect>());
    private Collider _collider = null;
    private IFieldBarrierEffect[] _effects = null;

    
    
    
    
    void Awake() {
        effects = GetComponents<IFieldBarrierEffect>();
    }
    
    
    
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

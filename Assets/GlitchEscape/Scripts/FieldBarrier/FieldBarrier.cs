using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldBarrier : MonoBehaviour {
    public enum CollisionType {
        Trigger,
        Collider
    };
    public CollisionType collisionType;
    public bool isTrigger {
        get { return collisionType == CollisionType.Trigger; }
        set {
            collisionType = value ? CollisionType.Trigger : CollisionType.Collider;
            collider.isTrigger = value;
        }
    }
    private Collider collider => _collider ?? Enforcements.GetComponent(this, out _collider);
    private IFieldBarrierEffect[] effects => _effects ?? Enforcements.GetComponents(this, out _effects);
    
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

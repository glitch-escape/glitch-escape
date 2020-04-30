using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InjectComponent : System.Attribute {
    
}


public class AutoWireTest : MonoBehaviour {
    [InjectComponent] public new Camera camera;
    [InjectComponent] public Player player;
    [InjectComponent] public new Rigidbody rigidbody;
    [InjectComponent] public new Collider collider;
}

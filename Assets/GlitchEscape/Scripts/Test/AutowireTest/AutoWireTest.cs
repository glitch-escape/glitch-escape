using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InjectComponent : System.Attribute {
    
}


public class AutoWireTest : MonoBehaviour {
    [InjectComponent] public Camera camera;
    [InjectComponent] public Player player;
    [InjectComponent] public Rigidbody rigidbody;
    [InjectComponent] public Collider collider;
}

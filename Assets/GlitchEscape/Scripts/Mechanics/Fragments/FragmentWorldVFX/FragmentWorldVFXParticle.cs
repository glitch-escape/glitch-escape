using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragmentWorldVFXParticle : MonoBehaviour {
    public Vector3 origin;
    public float offset;
    public float speed;
    public float yOffset;
    
    public void Init(float yOffset, float offset, float speed) {
        this.origin = transform.position;
        this.yOffset = yOffset;
        this.offset = offset;
        this.speed = speed;
    }
    public void Animate(float t) {
        transform.position = origin + yOffset * Mathf.Sin((t + offset) * speed) * Vector3.up;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fragment : MonoBehaviour {
    void Start() {
        // randomize fragment vfx
        GetComponent<Renderer>().material.SetFloat("Seed", Random.Range(-1000f, 0f));
    }
}

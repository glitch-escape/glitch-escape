using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingJumpWalls : MonoBehaviour
{
    Vector3 rotatable;
    // Start is called before the first frame update
    void Start()
    {
        this.rotatable = new Vector3(0f, 1f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        this.gameObject.transform.Rotate(rotatable, 1, Space.Self);
    }
}

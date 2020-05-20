using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PossibleRotation : MonoBehaviour
{
    bool rotate = false;
    Vector3 rotatable = new Vector3(0f, 1f, 0f);

    public void flickIt() { rotate = !rotate; }

    private void Update()
    {
        if(rotate) {
            this.gameObject.transform.Rotate(rotatable, 2, Space.Self);
        }
    }
}

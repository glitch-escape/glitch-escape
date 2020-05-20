using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PossibleRotation : MonoBehaviour
{
    bool rotate = false;
    Vector3 rotatable = new Vector3(0f, 1f, 0f);
    int degrees = 0;
    int howMuchToRotate = 2;

    public void flickIt() { rotate = !rotate; degrees = 0; }

    private void Update()
    {
        if(rotate && degrees != 45) {
            this.gameObject.transform.Rotate(rotatable, howMuchToRotate, Space.Self);
            degrees += howMuchToRotate;
            if(degrees >= 45)
            {
                rotate = false;
            }
        }

        if(!rotate && degrees >= 45)
        {
            degrees = 0;
        }
    }
}

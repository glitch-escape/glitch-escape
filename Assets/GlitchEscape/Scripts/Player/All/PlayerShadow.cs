using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShadow : MonoBehaviour
{
    [InjectComponent] public Player player;
    public GameObject landingIndicator;
    private Vector3 displayOffset;
    
    // Start is called before the first frame update
    void Start()
    {
        landingIndicator = Instantiate(landingIndicator);
        DontDestroyOnLoad(landingIndicator);
        landingIndicator.SetActive(false);
        displayOffset = new Vector3(0.0f, 0.01f, 0.0f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (GetFloorRaycast())
        {
            if (surfaceHit.distance > 0.5f)
            {
                //display the object on the surface and move it
                landingIndicator.SetActive(true);
                landingIndicator.transform.position = surfaceHit.point + displayOffset;
                float distanceScale = (Mathf.Log(surfaceHit.distance + 1.0f)) / 2.0f;
                landingIndicator.transform.localScale = new Vector3(distanceScale, 0f, distanceScale);
            }
            else
            {
                landingIndicator.SetActive(false);
            }
        }
        else
        {
            landingIndicator.SetActive(false);
        }
    }

    private RaycastHit surfaceHit;
    private bool GetFloorRaycast()
    {
        return Physics.Raycast(
            player.transform.position,
            Vector3.down,
            out surfaceHit,
            Mathf.Infinity);
    }
}

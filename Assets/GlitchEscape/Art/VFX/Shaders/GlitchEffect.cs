using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlitchEffect : MonoBehaviour
{
    protected Material m;
    private float animateTime = 1f;
    public float animateLength = 5f;

    // Start is called before the first frame update
    void Start()
    {
        m = this.gameObject.GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        m.SetFloat("_AlphaThreshold", animateTime);
        if(animateTime > -1)
        {
            animateTime -= Time.deltaTime / animateLength;
        }
        else
        {
            animateTime = -1;
        }
    }
}

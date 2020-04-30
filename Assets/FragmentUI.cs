using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragmentUI : MonoBehaviour
{
    public GameObject shardsPickups;
    public GameObject orbsPickups;
    List<GameObject> fragmentPieces = new List<GameObject>();
    // Start is called before the first frame update
    void Awake()
    {
        List<GameObject> orbIndicators = new List<GameObject>();
        GameObject fragmentUIIndicator = null;
        foreach (Transform child in transform)
        {
            if (child.gameObject.CompareTag("orbUI"))
                orbIndicators.Add(child.gameObject);
            else
                fragmentUIIndicator = child.gameObject;
        }

        fragmentPieces = new List<GameObject>();
        GameObject fragmentHolder = null;
        foreach(Transform child in fragmentUIIndicator.transform) 
        {
            if(!child.gameObject.CompareTag("FragBG"))
            {
                fragmentHolder = child.gameObject;
            }
        }

        foreach(Transform child in fragmentHolder.transform)
        {
            fragmentPieces.Add(child.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

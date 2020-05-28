using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragmentUI : MonoBehaviour
{
    GameObject shardsPickups;
    GameObject orbsPickups;
    List<GameObject> fragmentPieces = new List<GameObject>();

    private Player _player = null;
    private Player player => _player ?? Enforcements.GetSingleComponentInScene<Player>(this);

    bool sceneHasFragments;

    void Awake()
    {
        sceneHasFragments = FindObjectOfType<Fragment>() != null;
        if(sceneHasFragments)
        {
            List<GameObject> orbIndicators = new List<GameObject>();
            GameObject fragmentUIIndicator = null;
            foreach (Transform child in transform)
            {
                if (child.gameObject.CompareTag("OrbUI") && child.gameObject.activeInHierarchy)
                    orbIndicators.Add(child.gameObject);
                else if (child.gameObject.activeInHierarchy)
                    fragmentUIIndicator = child.gameObject;
            }

            //get reference to fragment shards UI
            fragmentPieces = new List<GameObject>();
            GameObject fragmentHolder = null;
            foreach (Transform child in fragmentUIIndicator.transform)
            {
                if (!child.gameObject.CompareTag("FragBG") && child.gameObject.activeInHierarchy)
                {
                    fragmentHolder = child.gameObject;
                }
            }

            if(fragmentHolder != null)
            {
                foreach (Transform child in fragmentHolder.transform)
                {
                    fragmentPieces.Add(child.gameObject);
                }
            }
        }
    }

    private void Update()
    {
        if(sceneHasFragments)
        {
            int fragsPickedUp = player.fragments.fragmentCount;
            for (int i = 0; i < fragsPickedUp; i++)
            {
                if (!fragmentPieces[i].activeInHierarchy)
                {
                    fragmentPieces[i].SetActive(true);
                }
            }

            if (fragsPickedUp == player.fragments.fragmentMax)
            {
                //activate orbui
            }
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragmentUI : PlayerComponent {
    GameObject shardsPickups;
    GameObject orbsPickups;
    private GameObject[] fragmentPieces;

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

            if(fragmentHolder != null) {
                fragmentPieces = fragmentHolder.GetComponentsInChildren<GameObject>();
                // foreach (Transform child in fragmentHolder.transform)
                // {
                //     fragmentPieces.Add(child.gameObject);
                // }
            }
        }
    }
    
    private void UpdateFragmentUI(Virtue activeVirtue, float fragmentCompletion) {
        int fragmentsPickedUp = (int) (fragmentCompletion * (float)fragmentPieces.Length);
        
        // TODO: switch fragment images depending on active virtue, etc.
        if (activeVirtue == Virtue.None) {
            fragmentsPickedUp = 0;
        }
        for (int i = 0; i < fragmentPieces.Length; ++i) {
            fragmentPieces[i].SetActive(i < fragmentsPickedUp);
        }
    }
    private void OnEnable() {
        player.fragments.onActiveVirtueChanged += OnVirtueTypeChanged;
        player.fragments.onFragmentPickedUp += OnFragmentPickedUp;
        var activeVirtue = player.fragments.activeVirtueInThisScene;
        UpdateFragmentUI(activeVirtue, player.fragments.GetFragmentCompletion(activeVirtue));
    }
    private void OnDisable() {
        player.fragments.onActiveVirtueChanged -= OnVirtueTypeChanged;
        player.fragments.onFragmentPickedUp -= OnFragmentPickedUp;
    }
    void OnVirtueTypeChanged(Virtue virtue) {
        Debug.Log("Changed active virtue for fragment pickups to " + virtue);
        UpdateFragmentUI(virtue, player.fragments.GetFragmentCompletion(virtue));
    }
    void OnFragmentPickedUp(PlayerVirtueFragments.FragmentInfo fragment) {
        Debug.Log("Picked up fragment for " + fragment.virtue);
        if (fragment.virtue == player.fragments.activeVirtueInThisScene) {
            UpdateFragmentUI(Virtue.Courage, player.fragments.GetFragmentCompletion(fragment.virtue));
        }
        /// TODO: can play fragment pickup animation / etc here
    }
    void OnVirtueCompleted(Virtue virtue) {
        Debug.Log("Picked up all fragments for " + virtue + "!");
        /// TODO: can play virtue completion animation / etc here
    }

}

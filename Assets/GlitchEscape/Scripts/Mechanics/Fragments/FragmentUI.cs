using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FragmentUI : PlayerComponent {
    GameObject shardsPickups;
    GameObject orbsPickups;
    private Transform[] fragmentPieces;

    private Player _player = null;
    private Player player => _player ?? Enforcements.GetSingleComponentInScene<Player>(this);

    List<GameObject> orbHolders = new List<GameObject>();

    bool sceneHasFragments;
    Transform humanityUI;
    Transform courageUI;
    Transform transUI;

    private void Awake()
    {
        Transform parentTrans = transform.parent;

        foreach (Transform child in parentTrans)
        {
            if (child.gameObject.CompareTag("OrbUI"))
            {
                orbHolders.Add(child.gameObject);
            }
        }

        foreach(GameObject holder in orbHolders)
        {
            foreach(Transform child in holder.transform)
            {
                if (child.gameObject.CompareTag("Humanity"))
                {
                    humanityUI = child;
                    humanityUI.gameObject.SetActive(false);
                }
                else if (child.gameObject.CompareTag("Courage"))
                {
                    courageUI = child;
                    courageUI.gameObject.SetActive(false);
                }
                else if (child.gameObject.CompareTag("Transcendence"))
                {
                    transUI = child;
                    transUI.gameObject.SetActive(false);
                }
            }
        }
    }

    private void OnLevelLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        var activeVirtue = player.fragments.activeVirtueInThisScene;
        UpdateFragmentUI(activeVirtue, player.fragments.GetFragmentCompletion(activeVirtue));
    }

    private void UpdateFragmentUI(Virtue activeVirtue, float fragmentCompletion) {
        // TODO: switch fragment images depending on active virtue, etc.
        
        // if fragmentPieces don't exist (for whatever reason - see above), skip
        if (fragmentPieces == null) return;
        
        // update fragment UI parts currently visible
        int fragmentsPickedUp = (int) (fragmentCompletion * (float)fragmentPieces.Length);
        /*if (activeVirtue == Virtue.None) {
            fragmentsPickedUp = 0;
        }*/
        for (int i = 1; i < fragmentPieces.Length; ++i) {
            fragmentPieces[i].gameObject.SetActive(i <= fragmentsPickedUp);
        }
    }
    private void OnEnable() {
        player.fragments.onActiveVirtueChanged += OnVirtueTypeChanged;
        player.fragments.onFragmentPickedUp += OnFragmentPickedUp;
        var activeVirtue = player.fragments.activeVirtueInThisScene;
        SceneManager.sceneLoaded += OnLevelLoaded;
        UpdateFragmentUI(activeVirtue, player.fragments.GetFragmentCompletion(activeVirtue));
    }
    private void OnDisable() {
        player.fragments.onActiveVirtueChanged -= OnVirtueTypeChanged;
        player.fragments.onFragmentPickedUp -= OnFragmentPickedUp;
        SceneManager.sceneLoaded -= OnLevelLoaded;
    }
    void OnVirtueTypeChanged(Virtue virtue) {
        Debug.Log("Changed active virtue for fragment pickups to " + virtue);
        Virtue sceneVirtue = player.fragments.activeVirtueInThisScene;
        sceneHasFragments = FindObjectOfType<Fragment>() != null;
        Debug.Log("Scenevirtue " + sceneVirtue);
        if (sceneHasFragments)
        {
            string virtueTag = "Blank";
            if (sceneVirtue == Virtue.Courage)
                virtueTag = "Courage";
            else if (sceneVirtue == Virtue.Humanity)
                virtueTag = "Humanity";
            else if (sceneVirtue == Virtue.Transcendence)
                virtueTag = "Transcendence";

            GameObject fragmentUIIndicator = null;
            GameObject fragmentUIBackground = null;

            foreach (Transform child in transform)
            {
                if (child.gameObject.CompareTag(virtueTag))
                {
                    if (child.childCount != 0)
                    {
                        Debug.Log(virtueTag + " active");
                        fragmentUIIndicator = child.gameObject;
                    }
                    else
                    {
                        Debug.Log(virtueTag + " active");
                        fragmentUIBackground = child.gameObject;
                    }
                }
                else
                {
                    child.gameObject.SetActive(false);
                }
            }
            fragmentUIIndicator.SetActive(true);
            fragmentUIBackground.SetActive(true);

            fragmentPieces = fragmentUIIndicator?.GetComponentsInChildren<Transform>() ?? null;
        }
        UpdateFragmentUI(virtue, player.fragments.GetFragmentCompletion(virtue));
    }
    void OnFragmentPickedUp(PlayerVirtueFragments.FragmentInfo fragment) {
        Debug.Log("Picked up fragment for " + fragment.virtue);
        if (fragment.virtue == player.fragments.activeVirtueInThisScene) {
            UpdateFragmentUI(fragment.virtue, player.fragments.GetFragmentCompletion(fragment.virtue));
        }
        
        int fragmentsPickedUp = (int)(player.fragments.GetFragmentCompletion(fragment.virtue) * (float)fragmentPieces.Length);
        if (fragmentsPickedUp >= 7)
        {
            OnVirtueCompleted(fragment.virtue);
        }
        /// TODO: can play fragment pickup animation / etc here
    }
    void OnVirtueCompleted(Virtue virtue) {
        Debug.Log("Picked up all fragments for " + virtue + "!");
        /// TODO: can play virtue completion animation / etc here
        
        if(virtue == Virtue.Humanity)
        {
            humanityUI.gameObject.SetActive(true);
        }
        else if (virtue == Virtue.Courage)
        {
            courageUI.gameObject.SetActive(true);
        }
        else if (virtue == Virtue.Transcendence)
        {
            transUI.gameObject.SetActive(true);
        }
    }

}

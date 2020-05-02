using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OrbUIController : MonoBehaviour
{
    private Player _player = null;
    private Player player => _player ?? Enforcements.GetSingleComponentInScene<Player>(this);

    private Animation orbGetAnim;

    private bool animPlayed = false;

    public TextMeshProUGUI returnToPortalPrompt;

    public InteractablePortal portal;

    private void Awake()
    {
        orbGetAnim = GetComponent<Animation>();
        returnToPortalPrompt.gameObject.SetActive(false);
    }

    private void Update()
    {
        if(!animPlayed && player.fragments.fragmentCount == 7)
        {
            orbGetAnim.Play();
            animPlayed = true;
            returnToPortalPrompt.gameObject.SetActive(true);
            portal.OpenPortal();
        }
    }
}

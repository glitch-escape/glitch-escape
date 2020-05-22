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
    public bool activatePortal = false;

    public TextMeshProUGUI returnToPortalPrompt;

    public Portal portal;
    public GameObject rootPortalObject;

    private void Awake()
    {
        orbGetAnim = GetComponent<Animation>();
        returnToPortalPrompt.gameObject.SetActive(false);
    }

    private void Update()
    {
        if(activatePortal || !animPlayed && player.fragments.fragmentCount >= 7)
        {
            if (rootPortalObject != null) {
                rootPortalObject.SetActive(true);
            }
            orbGetAnim.Play();
            animPlayed = true;
            returnToPortalPrompt.gameObject.SetActive(true);
            portal.active = true;
        }
    }
}

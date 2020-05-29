using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class FragmentPickupManager : PlayerComponent
{
    public int fragmentMax => player.config.fragmentTotal;
    public int fragmentCount;

    private void Start()
    {
        FragmentPickup[] allFragments = FindObjectsOfType<FragmentPickup>();
        player.config.fragmentTotal = allFragments.Length;
    }
    
    public void PickUpFragment(FragmentPickup someFragment)
    {
        fragmentCount++;
        FireEvent(PlayerEvent.Type.FragmentPickup);
    }

    //guess this should be used in the animation controller or sound controller instead?
    private void OnPlayerEvent(PlayerEvent.Type eventType)
    {
        if (eventType == PlayerEvent.Type.FragmentPickup)
        {
            //guess this should be in animation + sound controller?
            //play some animation or sound?
        }
    }
}
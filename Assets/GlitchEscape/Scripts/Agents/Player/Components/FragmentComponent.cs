using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class FragmentComponent : PlayerComponent
{
    public int fragmentMax => player.config.fragmentTotal;
    public int fragmentCount;

    private void Start()
    {
        FragmentComponent[] allFragments = FindObjectsOfType<FragmentComponent>();
        int fragmentTotal = allFragments.Length;
    }
    
    public void PickUpFragment(FragmentInteraction someFragment)
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
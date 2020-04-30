using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class FragmentComponent : PlayerComponent
{
    public int fragmentMax;
    public int fragmentCount;

    private void Start()
    {
        fragmentMax = player.config.fragmentTotal;
        if (fragmentMax == null || fragmentMax == 0)
        {
            fragmentMax = 10;
        }
        if (fragmentCount == null || fragmentCount == 0)
        {
            fragmentCount = 0;
        }
    }

    public void PickUpFragment(FragmentInteraction someFragment)
    {
        fragmentCount++;
    }

    private void OnPlayerEvent(PlayerEvent.Type eventType)
    {
        if (eventType == PlayerEvent.Type.FragmentPickup)
        {

            //guess this should be in animation + sound controller?
            //play some animation or sound?
        }
    }
}

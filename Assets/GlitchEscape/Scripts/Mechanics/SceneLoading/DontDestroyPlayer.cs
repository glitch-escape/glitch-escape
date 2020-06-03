using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyPlayer : MonoBehaviour
{
    [InjectComponent] public Player player;
    //GameObject thePlayer = player.get
    void Awake()
    {
        //FragmentInteraction[] allFragments = FindObjectsOfType<FragmentInteraction>();
        DontDestroyOnLoad(FindObjectOfType<PlayerController>());
    }
}
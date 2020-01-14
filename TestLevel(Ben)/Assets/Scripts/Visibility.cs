using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Visibility : MonoBehaviour
{
    public GameObject maze;
    public GameObject glitchMaze;
    public static bool disabled = false;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (disabled)
        {
            maze.SetActive(false);
            glitchMaze.SetActive(true);
        }
        else
        {
            maze.SetActive(true);
            glitchMaze.SetActive(false);
        }
            
    }
}

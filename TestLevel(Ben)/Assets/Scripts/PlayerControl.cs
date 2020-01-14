using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public int moveSpeed = 10;
    private float timer;
    private Vector3 switchPos;
    private bool atBase = true;
    private bool collided = false;
    private bool timerActive = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * Input.GetAxis("Vertical") * moveSpeed);
        transform.Translate(Vector3.right * Time.deltaTime * Input.GetAxis("Horizontal") * moveSpeed);

        if (collided && Input.GetKeyDown("space"))
        {
            Visibility.disabled = !Visibility.disabled;
            atBase = !atBase;
            // Activate timer if the player is in the glitch world
            if (!atBase)
            {
                switchPos = transform.position;
                timer = 7.0f;
                timerActive = true;
            }

            if (atBase)
            {
                timerActive = false;
            }
        }

        // Timer is counting down
        if (timer >= 0.0f && timerActive)
        {
            timer -= Time.deltaTime;
        }

        // This triggers when the timer runs out
        if (timer < 0.0f && timerActive)
        {
            timer = 0.0f;
            timerActive = false;
            Visibility.disabled = !Visibility.disabled;
            atBase = !atBase;
            transform.position = switchPos;
        }
    }

    // Check if player is standing on a switch
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Switch")
        {
            collided = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Switch")
        {
            collided = false;
        }
    }
}
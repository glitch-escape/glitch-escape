using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class PlayerManifestController : MonoBehaviour, IPlayerControllerComponent
{
    const float GRAVITY = 9.81f; // m/s^2

    private PlayerController controller;
    private Player player;
    private Input input;
    private Animator animator;
    private new Rigidbody rigidbody;
    private List<Material> defaultMaterials;
    private Renderer[] renderers;
    //public Material glitchMaterial;

    public void SetupControllerComponent(PlayerController controller)
    {
        this.controller = controller;
        player = controller.player;
        rigidbody = player.rigidbody;
        animator = player.animator;
        input = player.input;
        input.Controls.Manifest.performed += context => {
            bool pressed = context.ReadValue<float>() > 0f;
            manifestPressed = pressed;
        };
        renderers = GetComponentsInChildren<Renderer>();
        defaultMaterials = new List<Material>();
        foreach (var renderer in renderers)
        {
            foreach (var material in renderer.materials)
            {
                defaultMaterials.Add(material);
            }
        }
        //animator.SetBool("isDashing", false);
    }

    public float manifestStaminaCost = 10f;

    // is the manifest button currently pressed?
    private bool manifestPressed = false;

    private bool isVfxActive = false;
    public float manifestVfxDuration = 1.2f;
    //wall prefab reference
    public GameObject manifestWall;

    #region ManifestMechanics
    #region ScriptProperties

    [Range(0f, 2f)]
    [Tooltip("minimum time between manifests")]
    public float manifestCooldown = 0.2f;

    [Range(0f, 2f)]
    [Tooltip("length of time that manifest is active")]
    public float manifestDuration = 0.5f;

    #endregion
    #region PrivateVariablesAndDerivedProperties

    // is player currently manifesting?
    private bool isManifesting = false;

    // was manifest button currently pressed as of last frame?
    private bool isManifestPressed = false;

    // time that manifest started (seconds)
    private float manifestStartTime = -10f;

    // time that manifest has been pressed so far
    private float manifestPressTime = 0f;

    //current instance of manifest wall
    private GameObject thisManifestWall;

    // time that manifest has been active so far
    private float elapsedManifestTime
    {
        get { return Time.time - manifestStartTime; }
    }
    #endregion
    #region ManifestImplementation

    public void Update()
    {
        // handle manifest press input
        if (manifestPressed != isManifestPressed)
        {
            // Debug.Log("manifest state changed! "+ismanifestPressed+" => "+manifestPressed);
            if (manifestPressed)
            {
                isManifestPressed = true;
                BeginManifest();
            }
            else
            {
                // man button released
                isManifestPressed = false;
            }
        }
        else if (isManifestPressed)
        {
            // update man press time
            manifestPressTime = Time.time - manifestPressTime;
        }

        if (Time.time > manifestStartTime + manifestDuration)
        {
            EndManifest();
        }
        if (Time.time > manifestStartTime + manifestVfxDuration)
        {
            isVfxActive = false;
        }

        // move the player if they're currently manifesting, and update vfx
        if (isManifesting)
        {
            //manifest it

        }
        if (isVfxActive)
        {
            // update vfx
        }
    }
    private void BeginManifest()
    {
        // do we have enough stamina to perform this action? if no, cancel
        if (!player.TryUseAbility(manifestStaminaCost))
        {
            return;
        }

        // check: can we manifest yet? if no, cancel
        if (Time.time < manifestStartTime + manifestCooldown)
        {
            return;
        }

        // if already manifesting, end that + restart
        if (isManifesting)
        {
            EndManifest();
        }
        /*if (!animator.GetBool("isDashing"))
        {
            Debug.Log("starting dash animation");
            animator.SetBool("isDashing", true);
            animator.SetTrigger("startDashing");
        }*/
        // begin manifest
        thisManifestWall = Instantiate(manifestWall, transform.position, transform.rotation);
        thisManifestWall.transform.position = player.transform.position + (2*player.transform.forward) + Vector3.up;
        thisManifestWall.transform.rotation = player.transform.rotation;
        // Debug.Log("Start manifest!");
        //BeginmanifestVfx();
        isVfxActive = true;

        isManifesting = true;
        manifestStartTime = Time.time;
    }

    private void EndManifest()
    {
        /*if (animator.GetBool("isDashing"))
        {
            Debug.Log("ending dash animation");
            animator.SetBool("isDashing", false);
            animator.SetTrigger("stopDashing");
        }*/
        if (isManifesting)
        {
            //end manifest
            Destroy(thisManifestWall);
        }
        isManifesting = false;
    }
    #endregion
    #endregion
}

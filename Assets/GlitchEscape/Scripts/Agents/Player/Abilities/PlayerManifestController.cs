using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class PlayerManifestController : PlayerAbility {
    /// <summary>
    /// Player reference
    /// </summary>

    public override float resourceCost => player.config.manifestAbilityStaminaCost;
    public override float cooldownTime => player.config.manifestAbilityCooldownTime;
    protected override float abilityDuration => player.config.manifestAbilityShieldDuration;
    
    protected override void AbilityStart() {
        BeginManifest();
        FireEvent(PlayerEvent.Type.BeginManifest);
    }
    protected override void AbilityUpdate() {
        if (elapsedTime > manifestVfxDuration)
        {
            isVfxActive = false;
        }
    }
    protected override void AbilityEnd() {
        EndManifest();
        FireEvent(PlayerEvent.Type.EndManifest);
    }
    protected override void ResetAbility() {
        if (isManifesting) EndManifest();
        isVfxActive = false;
    }
    private List<Material> defaultMaterials;
    private Renderer[] renderers;

    void Awake () {
        renderers = GetComponentsInChildren<Renderer>();
        defaultMaterials = new List<Material>();
        foreach (var renderer in renderers)
        {
            foreach (var material in renderer.materials)
            {
                defaultMaterials.Add(material);
            }
        }
    }
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

    //current instance of manifest wall
    private GameObject thisManifestWall;

    #endregion
    #region ManifestImplementation

    protected override PlayerControls.HybridButtonControl inputButton => PlayerControls.instance.manifest;
    private void BeginManifest() {
        // begin manifest
        thisManifestWall = Instantiate(manifestWall, transform.position, transform.rotation);
        thisManifestWall.transform.position = player.transform.position + (2*player.transform.forward) + Vector3.up;
        thisManifestWall.transform.rotation = player.transform.rotation;
        // Debug.Log("Start manifest!");
        //BeginmanifestVfx();
        isVfxActive = true;
        isManifesting = true;
    }

    private void EndManifest() {
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DodgeScript : MonoBehaviour
{
    private Input input;
    private Rigidbody m_Rigidbody;

    public float dodgeMaxDistance = 1.0f;
    private float dodgeHoldTime = 0.0f;
    private bool holdingDodge = false;
    public float dodgeScaleFactor = 15f;
    public ParticleSystem dodgeGroundParticle;
    public ParticleSystem dodgeAirParticle;

    private Material defaultMaterial;
    private Shader dodgeShader;
    private Shader defaultShader;
    private float animateTime = 1.0f;
    public float animateLength = .01f;
    public Texture noiseTex;
    
    void Awake()
    {
        input = new Input();
        m_Rigidbody = this.GetComponent<Rigidbody>();

        defaultMaterial = this.transform.Find("Body").GetComponent<Renderer>().material;
        dodgeShader = Shader.Find("Custom/TeleportEffect");
        defaultShader = Shader.Find("Custom/Toon");
    }

    private void OnEnable() => input.Controls.Enable();

    private void OnDisable() => input.Controls.Disable();

    void Update()
    {
        if (input.Controls.Dodge.triggered && !holdingDodge)
        {
            holdingDodge = true;
            dodgeGroundParticle.Emit(1);
            defaultMaterial.shader = dodgeShader;
            defaultMaterial.SetTexture("_Noise", noiseTex);
        }
        else if (input.Controls.Dodge.triggered && holdingDodge)
        {
            holdingDodge = false;
            Dodge();
            dodgeHoldTime = 0f;
            defaultMaterial.shader = defaultShader;
            animateTime = 1.0f;
        }

        if (holdingDodge)
        {
            defaultMaterial.SetFloat("_AlphaThreshold", animateTime);
            dodgeHoldTime += Time.deltaTime;
            dodgeGroundParticle.transform.position = this.transform.position + (-transform.forward * dodgeHoldTime * dodgeScaleFactor);
            if (animateTime > -1)
            {
                animateTime -= Time.deltaTime / animateLength;
            }
            else
            {
                animateTime = -1;
            }
        }
    }


    void Dodge()
    {
        //Vector3 desiredBackward = Vector3.RotateTowards(-transform.forward, cameraDir, turnSpeed * Time.deltaTime, 0f);
        //dodgeGroundParticle.Emit(1);
        dodgeAirParticle.Emit(30);
        Vector3 desiredBackward = -transform.forward * dodgeHoldTime * dodgeScaleFactor;
        //this.gameObject.transform.position += desiredBackward * dodgeHoldTime * 5;
        m_Rigidbody.MovePosition(this.transform.position + desiredBackward);
        //m_Rigidbody.AddForce(desiredBackward);
    }
}

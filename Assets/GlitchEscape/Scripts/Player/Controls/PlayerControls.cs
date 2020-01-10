using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    public float turnSpeed = 10f;
    public float jumpHeight = 1f;

    private Animator m_Animator;
    private Rigidbody m_Rigidbody;
    private Vector3 m_Movement;
    private Quaternion m_Rotation = Quaternion.identity;
    private Input input;
    private Vector2 movementInput;
    private bool inInteraction = false;

    void Awake()
    {
        input = new Input();
        m_Animator = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();
    }
    private void OnEnable() => input.Controls.Enable();
    private void OnDisable() => input.Controls.Disable();

    void FixedUpdate() => Move();

    private void Move()
    {
        var movementInput = input.Controls.Move.ReadValue<Vector2>();

        float horizontal = movementInput.x;
        float vertical = movementInput.y;

        m_Movement.Set(horizontal, 0f, vertical);
        m_Movement.Normalize();

        bool hasHorizontalInput = !Mathf.Approximately(horizontal, 0f);
        bool hasVerticalInput = !Mathf.Approximately(vertical, 0f);
        bool isWalking = hasHorizontalInput || hasVerticalInput;

        m_Animator.SetBool("IsWalking", isWalking);

        Vector3 desiredForward = Vector3.RotateTowards(transform.forward, m_Movement, turnSpeed * Time.deltaTime, 0f);
        m_Rotation = Quaternion.LookRotation(desiredForward);
    }

    void OnAnimatorMove()
    {
        m_Rigidbody.MovePosition(m_Rigidbody.position + m_Movement * m_Animator.deltaPosition.magnitude);
        m_Rigidbody.MoveRotation(m_Rotation);
    }

    public void Jump() // disabled
    {
        transform.position = new Vector3(transform.position.x, transform.position.y + jumpHeight, transform.position.z);
    }

    public void Interact()
    {
        if(inInteraction == true)
        {
            Debug.Log("switching levels");
        } 
        else
        {
            Debug.Log(inInteraction);
        }
    }

    void OnTriggerEnter (Collider other)
    {
        if (other.tag == "Intersection")
        {
            inInteraction = true;
        }
    }

    void OnTriggerExit (Collider other)
    {
        if (other.tag == "Intersection")
        {
            inInteraction = false;
        }
    }

}

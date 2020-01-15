using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerControls : MonoBehaviour
{
    public GameObject maze;
    public GameObject glitchMaze;

    public float turnSpeed = 10f;
    public float jumpHeight = 1f;

    private Animator m_Animator;
    private Rigidbody m_Rigidbody;
    private Vector3 m_Movement;
    private Quaternion m_Rotation = Quaternion.identity;
    private Input input;
    private Vector2 movementInput;
    private static bool onSwitch = false;
    public float moveSpeed = 1.0f;
    public Transform cameraTarget;
    
    void Awake()
    {
        input = new Input();
        m_Animator = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    private void OnEnable() => input.Controls.Enable();

    private void OnDisable() => input.Controls.Disable();

    void FixedUpdate() {
        Move();
        if (transform.position.y < -10f) {
            SceneManager.LoadScene("SampleScene");
        }
    }

    private void Move()
    {
        var movementInput = input.Controls.Move.ReadValue<Vector2>();

        float horizontal = movementInput.x;
        float vertical = movementInput.y;

        m_Movement.Set(horizontal, 0f, vertical);
        // m_Movement.Normalize();

        bool hasHorizontalInput = !Mathf.Approximately(horizontal, 0f);
        bool hasVerticalInput = !Mathf.Approximately(vertical, 0f);
        bool isWalking = hasHorizontalInput || hasVerticalInput;

        m_Animator.SetBool("IsWalking", isWalking);

        Vector3 desiredForward = Vector3.RotateTowards(transform.forward, m_Movement, turnSpeed * Time.deltaTime, 0f);
        m_Rotation = Quaternion.LookRotation(desiredForward);

        // var cameraDir = cameraTarget.position - transform.position;
        // cameraDir.z = 0;
        // var rot = Quaternion.FromToRotation(cameraDir, Vector3.forward);
        
        transform.Translate(m_Movement * Time.deltaTime * moveSpeed);
        
        
    }

    void OnAnimatorMove()
    {
        m_Rigidbody.MovePosition(m_Rigidbody.position + m_Movement * m_Animator.deltaPosition.magnitude);
        m_Rigidbody.MoveRotation(m_Rotation);
    }

    public void OnJump() // disabled
    {
        // transform.position = new Vector3(transform.position.x, transform.position.y + jumpHeight, transform.position.z);
    }

    public void OnInteract()
    {
        if (onSwitch == true)
        {
            maze.SetActive(!maze.activeInHierarchy);
            glitchMaze.SetActive(!glitchMaze.activeInHierarchy);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Switch")
        {
            onSwitch = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Switch")
        {
            onSwitch = false;
        }
    }

}

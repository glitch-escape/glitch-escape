﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour{
    public GameObject maze;
    public GameObject glitchMaze;
    public Transform camera;

    public Vector3 debugtext;

    public float turnSpeed = 10f;
    public float jumpHeight = 1f;

    private Animator m_Animator;
    private Rigidbody m_Rigidbody;
    public Vector3 m_Movement;
    private Vector3 c_Direction;
    private Quaternion m_Rotation = Quaternion.identity;
    private Input input;
    private Vector2 movementInput;
    private static bool onSwitch = false;

    void Awake()
    {
        input = new Input();
        m_Animator = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
    }

    private void OnEnable() => input.Controls.Enable();

    private void OnDisable() => input.Controls.Disable();

    void FixedUpdate() => Move();

    [System.Obsolete]
    void Update()
    {
        if (transform.position.y < -5)
        {
            Application.LoadLevel(Application.loadedLevel);
        }
    }

    private void Move()
    {
        var movementInput = input.Controls.Move.ReadValue<Vector2>();

        float horizontal = movementInput.x;
        float vertical = movementInput.y;

        bool hasHorizontalInput = !Mathf.Approximately(horizontal, 0f);
        bool hasVerticalInput = !Mathf.Approximately(vertical, 0f);
        bool isSprinting = hasHorizontalInput || hasVerticalInput;

        m_Animator.SetBool("isSprinting", isSprinting);

        m_Movement.Set(horizontal, 0f, vertical);
        m_Movement.Normalize();

        // c_Direction = camera.forward;
        // c_Direction.y = 0;
        // c_Direction.Normalize();

        var forward = camera.transform.forward;
        var right = camera.transform.right;

        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        c_Direction = forward * vertical + right * horizontal;

        Vector3 desiredForward = Vector3.RotateTowards(transform.forward, c_Direction, turnSpeed * Time.deltaTime, 0f);
        m_Rotation = Quaternion.LookRotation(desiredForward);
    }

    void OnAnimatorMove()
    {
        m_Rigidbody.MovePosition(m_Rigidbody.position + c_Direction * m_Animator.deltaPosition.magnitude);
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

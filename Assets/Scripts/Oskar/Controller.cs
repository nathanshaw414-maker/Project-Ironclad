using System;
using Mirror;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Controller : NetworkBehaviour
{
    [Header("Movement Parameters")]
    public float MaxSpeed => SprintInput ? sprintSpeed:walkSpeed;
    public float Acceleration = 15f;

    [SerializeField] float walkSpeed = 3.5f;
    [SerializeField] float sprintSpeed = 15f;


  



    [Header("Look Parameters")]
    public Vector2 LookSensitivity = new Vector2(0.1f, 0.1f);

    public float Pitchlimit = 85f;

    [SerializeField] float currentPitch = 0f;

    public float CurrentPitch
    {
        get=> currentPitch;

        set
        {
            currentPitch = Mathf.Clamp(value, -Pitchlimit, Pitchlimit);
        }
    }

    [Header("Physics Parameters")]
    [SerializeField] float Gravity = 3f;

    public float verticalVelocity = 0f;
    public Vector3 currentVelocity { get; private set; }
    public float CurrentSpeed { get; private set; }

    public bool IsGrounded => characterController.isGrounded;

    [SerializeField] float jumpHieght = 2f;

    [Header("Input")]
    public Vector2 moveInput;
    public Vector2 lookInput;
    public bool SprintInput;

    [Header("Components")]
    [SerializeField] Camera mainCamera;
    [SerializeField] CharacterController mainCharacterController;


    CharacterController characterController;


    
 

    private void Awake()
    {
        if (characterController == null)
        {
            characterController = GetComponent<CharacterController>();
        }
    }
    private void Update()
    {
        MoveUpdate();
        LookUpdate();
    }
    public void TryJump()
    {
        if (IsGrounded == false)
        {
            return;
        }
        verticalVelocity = Mathf.Sqrt(jumpHieght * -2f * Physics.gravity.y * Gravity);
    }

    private void MoveUpdate()
    {
        Vector3 motion = transform.forward * moveInput.y + transform.right * moveInput.x;
        motion.y = 0f;
        motion.Normalize();

        if (motion.sqrMagnitude >= 0.01f)
        { 
            currentVelocity = Vector3.MoveTowards(currentVelocity, motion * MaxSpeed, Acceleration * Time.deltaTime);
        }
        else
        {
            currentVelocity = Vector3.MoveTowards(currentVelocity, Vector3.zero, Acceleration * Time.deltaTime);
        }

        if (IsGrounded && verticalVelocity <= 0.1f)
        {
            verticalVelocity = -3f;
        }
        else
        {
            verticalVelocity += Physics.gravity.y * Gravity * Time.deltaTime;
        }
        Vector3 fullVelocity = new Vector3(currentVelocity.x, verticalVelocity, currentVelocity.z);


        characterController.Move(fullVelocity * Time.deltaTime);

        // update speed
        CurrentSpeed = currentVelocity.magnitude;
    }

    private void LookUpdate()
    {
        Vector2 input = new Vector2(lookInput.x * LookSensitivity.x, lookInput.y * LookSensitivity.y);
        //look up/down
        CurrentPitch -= input.y;

        mainCamera.transform.localRotation = Quaternion.Euler(CurrentPitch, 0f, 0f);

        //look left/right
        transform.Rotate(Vector3.up, input.x);


    }
    


}

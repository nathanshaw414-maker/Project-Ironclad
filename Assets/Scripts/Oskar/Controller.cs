using Mirror;
using System;
using System.Collections;
using System.Security.Principal;
using Unity.Services.Authentication;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using static UnityEditor.Progress;

[RequireComponent(typeof(CharacterController))]
public class Controller : NetworkBehaviour
{
    [Header("Movement Parameters")]
    public float MaxSpeed => SprintInput ? sprintSpeed : walkSpeed;
    public float Acceleration = 15f;

    [SerializeField] float walkSpeed = 3.5f;
    [SerializeField] float sprintSpeed = 10f;

    [SerializeField] float slowDownSpeed = 15f;
    [SerializeField] float slowDownSpeedRate = 1f;
    Vector3 moveDampVelocity;
    [SerializeField] float slowingDownVelocity;

    Vector3 dashVelocity;
    bool isDodging = false;
    [SerializeField] float currentDodgeDuration;
    [SerializeField] float dodgeDuration = 0.25f;
    [SerializeField] float dodgeStrength = 20f;

    [Header("Look Parameters")]
    public Vector2 LookSensitivity = new Vector2(0.1f, 0.1f);

    public float Pitchlimit = 85f;

    [SerializeField] float currentPitch = 0f;

    public float CurrentPitch
    {
        get => currentPitch;

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

    public bool IsGrounded; //=> characterController.isGrounded;

    [SerializeField] float jumpHieght = 2f;

    [Header("Input")]
    public Vector2 moveInput;
    public Vector2 lookInput;
    public bool SprintInput;

    [Header("Components")]
    [SerializeField] Camera mainCamera;
    [SerializeField] CharacterController mainCharacterController;
    [SerializeField] Equipment equipment;
    [SerializeField] Animator animator;
    [SerializeField] NetworkAnimator networkAnimator; 
    [SyncVar]
    bool isRunning;

    CharacterController characterController;

    [Header("Pickup")]
    [SerializeField] float pickupRange = 4f;



    private void Awake()
    {
        if (characterController == null)
        {
            characterController = GetComponent<CharacterController>();
        }
    }
    private void Update()
    {
        IsGrounded = Physics.Raycast(transform.position, Vector3.down, 0.2f);

        if (isLocalPlayer)
        {
            MoveUpdate();
            LookUpdate();
        }

        if (isDodging)
        {
            characterController.Move(dashVelocity * Time.deltaTime);
            if (currentDodgeDuration <= 0)
            {
                isDodging = false;
                
            }
            else
            {
                currentDodgeDuration -= Time.deltaTime;
            }
        }

    }
    public void TryJump()
    {
        if (IsGrounded == false)
        {
            //Debug.Log("Not grounded");
            return;
        }
        verticalVelocity = Mathf.Sqrt(jumpHieght * -2f * Physics.gravity.y * Gravity);
       
            networkAnimator.SetTrigger("Jump");

    }
    public void TryDodge()
    {
        Vector3 motion = transform.forward * moveInput.y + transform.right * moveInput.x;
        motion.y = 0f;
        motion.Normalize();
       
        dashVelocity = Vector3.MoveTowards(currentVelocity, motion * dodgeStrength, Acceleration *  10f);

        //characterController.Move(dashVelocity);
        currentDodgeDuration = dodgeDuration;
        isDodging = true;
            

    }


    private void MoveUpdate()
    {
        
        Vector3 motion = transform.forward * moveInput.y + transform.right * moveInput.x;
        motion.y = 0f;
        motion.Normalize();

        if (SprintInput)
        {
            slowingDownVelocity = slowDownSpeed * MaxSpeed;
        }
        slowingDownVelocity = Mathf.Clamp(slowingDownVelocity - slowDownSpeedRate * Time.deltaTime, slowDownSpeed * walkSpeed, slowDownSpeed * sprintSpeed);

        if (motion.sqrMagnitude >= 0.01f)
        {
            //currentVelocity = Vector3.MoveTowards(currentVelocity, motion * MaxSpeed, Acceleration * Time.deltaTime);

            currentVelocity = Vector3.SmoothDamp(currentVelocity, motion * MaxSpeed, ref moveDampVelocity, 0.1f);

            //animator.SetBool("isRunning", true);
            

        }
        else
        {
            //currentVelocity = new Vector3(0f, 0f, 0f);
            //currentVelocity = Vector3.MoveTowards(currentVelocity, Vector3.zero, Acceleration * Time.deltaTime);
            
            //currentVelocity = Vector3.SmoothDamp(currentVelocity, Vector3.zero, ref moveDampVelocity, 0.1f);
            
            currentVelocity = Vector3.MoveTowards(currentVelocity, Vector3.zero, slowingDownVelocity * Time.deltaTime);

           // animator.SetBool("isRunning", false);
           

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
        
        

        //if (isLocalPlayer)
        //{
            characterController.Move(fullVelocity * Time.deltaTime);
            animator.SetBool("isRunning", motion.sqrMagnitude >= 0.01f);
        //}
       
       // runAnimationStartServer(motion.sqrMagnitude >= 0.01f);

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


    public void attack()
    {
        if (equipment.Weapon1 != null)
        {
            equipment.Weapon1.GetComponent<Attack>().TargetRaycast();
        }
    }
    public void CmdPickup()
    {

        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out RaycastHit hit, pickupRange))
        {
            if (hit.transform.root.gameObject.tag == "Item")
            {

                if (equipment.Weapon1 != null)
                {

                    dropWeaponServer();


                }

                //GameObject temp = spawnWeapon(hit.transform.root.gameObject);
                despawnWeapon(hit.transform.root.gameObject, hit.transform.root.gameObject.GetComponent<Item>().GetItemID());
                // StartCoroutine(EquipWeapon(hit.transform.root.gameObject, hit.transform.root.gameObject.GetComponent<Item>().GetItemID()));
                // GameObject temp2 = spawnWeapon(temp);
                // despawnWeapon(temp);





                //equipment.Weapon1 = hit.transform.root.gameObject;

                //equipment.Weapon1.transform.root.tag = "Untagged";
                //equipment.Weapon1.transform.SetParent(this.transform);
                //equipment.Weapon1.GetComponent<Attack>().SetCamera(mainCamera);
                //equipment.Weapon1.SetActive(true);
                
            }

        }
    }
    [Command]
    private void despawnWeapon(GameObject wep, int ItemID)
    {


        Destroy(wep);
        GameObject ItemPrefab = FindFirstObjectByType<ItemManager>().getWeaponByID(ItemID);
        GameObject temp = Instantiate(ItemPrefab, transform.position, Quaternion.identity);



        NetworkServer.Spawn(temp, gameObject);

        equipWeapon(temp);





    }
    [ClientRpc]
    void equipWeapon(GameObject temp)
    {
        equipment.Weapon1 = temp;

        equipment.Weapon1.transform.root.tag = "Untagged";
        equipment.Weapon1.transform.SetParent(this.transform);
        equipment.Weapon1.GetComponent<Attack>().SetCamera(mainCamera);
        equipment.Weapon1.SetActive(true);
    }
    [ClientRpc]
    void dropWeapon()
    {
        equipment.Weapon1.transform.tag = "Item";
        //NetworkServer.Destroy(equipment.Weapon1);
        equipment.Weapon1.transform.SetParent(null);
    }
    [Command]
    void dropWeaponServer()
    {
        dropWeapon();
    }


  
}

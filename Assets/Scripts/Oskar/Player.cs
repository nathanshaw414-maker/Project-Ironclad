using Mirror;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Controller))]
public class Player : NetworkBehaviour
{
    [Header("Components")]
    [SerializeField] Controller characherController;

    [SerializeField] Camera myCamera;
    [SerializeField] PlayerInput playerInput;
    [SerializeField] Attack playerAttack;


    void OnMove(InputValue value)
    { 
        characherController.moveInput = value.Get<Vector2>();
    }
    void OnLook(InputValue value)
    {
        characherController.lookInput = value.Get<Vector2>();
    }
    void OnSprint(InputValue value)
    {
        characherController.SprintInput = value.isPressed;
    }
    void OnJump(InputValue value)
    {
        if (value.isPressed )
        {
            characherController.TryJump();
        }
    }

    void OnClick(InputValue value)
    {
        if (value.isPressed)
        {
            
            playerAttack.TargetRaycast();
        }
    }

 

    private void Start()
    {

        if (isLocalPlayer)
        {
            if (!myCamera.enabled) { myCamera.enabled = true; };
            if (!playerInput.enabled) { playerInput.enabled = true; };
        }
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}

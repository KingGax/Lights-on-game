using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Runtime.InteropServices;

public class PlayerInputScript : MonoBehaviour
{
    private PlayerController pc;
    private PlayerInputs inputController;
    private PlayerInputs.PlayerActions movementInputMap;

    [DllImport("__Internal")]
    private static extern string getMicInput();

    void Awake()
    {
        inputController = new PlayerInputs();


        movementInputMap = inputController.Player;

        movementInputMap.Movement.performed += ctx => OnMovement(ctx);
        movementInputMap.Movement.started += ctx => OnMovement(ctx);
        movementInputMap.Movement.canceled += ctx => OnMovement(ctx);
        movementInputMap.Dash.started += ctx => Dash(ctx);
        movementInputMap.Light.started += _ => ChangeLight();

        movementInputMap.Attack.started += ctx => AttackOne(ctx);
        movementInputMap.Attack.performed += ctx => AttackOne(ctx);

        movementInputMap.Voice.started += ctx => VoiceControl(ctx);
    }
    // Start is called before the first frame update
    void Start()
    {
        pc = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnEnable()
    {
        inputController.Enable();
    }

    private void OnDisable()
    {
        inputController.Disable();
    }
    void AttackOne(InputAction.CallbackContext ctx)
    {

        if (ctx.performed)
        {//performed in this case means released
            pc.AttackOne(false);
        }
        else
        {
            pc.AttackOne(true);

        }
    }
    void ChangeLight()
    {
        pc.ChangeLight();
    }
    void Dash(InputAction.CallbackContext ctx)
    {
        pc.Dash();
    }
    public void OnMovement(InputAction.CallbackContext ctx)
    {
        Vector2 newMovementInput = ctx.ReadValue<Vector2>();
        pc.OnMovement(newMovementInput);

        // Vector3 directionVector = transform.position - new Vector3(newMovementInput.x, 0, newMovementInput.y);
        // transform.LookAt(directionVector);
        //if (ctx.)
        //Vector3 forwardVector = 
        /*if (movement != Vector2.zero){
            transform.forward = cameraForward * movement.y  + cameraRight * movement.x;
        }*/

        //transform.Rotate(new Vector3(0, 30, 0), Space.World);
    }
    public void VoiceControl(InputAction.CallbackContext ctx)
    {
        string colour = (string) getMicInput();
        LanternColour colourEnum = new LanternColour
        if (colour == "RED")
            colourEnum = LanternColour.Red;
        else if (colour == "BLUE")
            colourEnum = LanternColour.Blue;
        else if (colour == "GREEN")
            colourEnum = LanternColour.Green;
        if (colourEnum != null)
            pc.ChangeLightToColour(colourEnum);
    }
}


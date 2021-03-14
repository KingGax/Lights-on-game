using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Runtime.InteropServices;

public class PlayerInputScript : MonoBehaviour {

    private PlayerController pc;
    private PlayerInputs inputController;
    private PlayerInputs.PlayerActions movementInputMap;
    private bool inputEnabled = true;
    private HelpTooltip helpView;

    [DllImport("__Internal")]
    private static extern void startRecogniser();

    void Awake() {
        inputController = new PlayerInputs();

        movementInputMap = inputController.Player;

        movementInputMap.Movement.performed += ctx => OnMovement(ctx);
        movementInputMap.Movement.started += ctx => OnMovement(ctx);
        movementInputMap.Movement.canceled += ctx => OnMovement(ctx);
        movementInputMap.Dash.started += ctx => Dash(ctx);
        movementInputMap.Light.started += _ => ChangeLight();
        movementInputMap.SwitchWeapon.started += _ => SwitchWeapon();


        movementInputMap.Attack.started += ctx => AttackOne(ctx);
        movementInputMap.Attack.performed += ctx => AttackOne(ctx);
        movementInputMap.AltAttack.started += ctx => AttackAlt(ctx);
        movementInputMap.AltAttack.performed += ctx => AttackAlt(ctx);

        movementInputMap.Voice.started += ctx => VoiceControl(ctx);
        movementInputMap.Help.started += ctx => ToggleHelpTooltip(ctx);
    }

    // Start is called before the first frame update
    void Start() {
        pc = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update() {

    }

    void SwitchWeapon() {
        pc.SwitchWeapon();
    }

    public void EnableInput() {
        inputEnabled = true;
    }

    public void DisableInput() {
        inputEnabled = false;
    }

    private void OnEnable() {
        inputController.Enable();
    }

    private void OnDisable() {
        inputController.Disable();
        pc.OnMovement(Vector2.zero);
    }

    void AttackOne(InputAction.CallbackContext ctx) {
        if (inputEnabled) {
            if (ctx.performed) {
                //performed in this case means released
                pc.AttackOne(false);
            }
            else {
                pc.AttackOne(true);
            }
        }
    }

    void AttackAlt(InputAction.CallbackContext ctx) {
        if (inputEnabled) {
            if (ctx.performed) {
                //performed in this case means released
                pc.AttackAlt(false);
            }
            else {
                pc.AttackAlt(true);
            }
        }
    }

    void ChangeLight() {
        if (inputEnabled) {
            pc.ChangeLight();
        }
    }

    void Dash(InputAction.CallbackContext ctx) {
        if (inputEnabled) {
            pc.Dash();
        }
    }

    public void OnMovement(InputAction.CallbackContext ctx) {
        if (inputEnabled) {
            Vector2 newMovementInput = ctx.ReadValue<Vector2>();
            pc.OnMovement(newMovementInput);
        }
        

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
        if (inputEnabled) {
            startRecogniser();
        }
    }
    public void ToggleHelpTooltip(InputAction.CallbackContext ctx) {
        helpView.ToggleVisibility();
    }
}


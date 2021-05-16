using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Runtime.InteropServices;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerInputScript : MonoBehaviour {

    private PlayerController pc;
    private PlayerInputs inputController;
    private PlayerInputs.PlayerActions movementInputMap;
    private bool inputEnabled = true;
    private bool cameraCutscene = false;
    public SpriteRenderer micRenderer;
    public float cameraCutsceneLength;
    private HelpTooltip helpView = null;
    private InGameMenu menuView = null;
    private PhotonView pv;
    private bool micRequestActive = false;
    CameraWork cameraWork;
    Animator cameraAnimator;

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
        movementInputMap.ChangePersp.started += _ => ChangePerspective();

        //movementInputMap.SwitchWeapon.started += _ => SwitchWeapon(); disabled for showcase


        movementInputMap.Attack.started += ctx => AttackOne(ctx);
        movementInputMap.Attack.performed += ctx => AttackOne(ctx);
        movementInputMap.AltAttack.started += ctx => AttackAlt(ctx);
        movementInputMap.AltAttack.performed += ctx => AttackAlt(ctx);

        movementInputMap.Voice.started += ctx => VoiceControl(ctx);
        movementInputMap.HelpToggle.started += ctx => ToggleHelpTooltip(ctx);
        movementInputMap.Pause.started += ctx => ToggleMenu(ctx);
        movementInputMap.Reload.started += _ => Reload();
        movementInputMap.SpectatorToggle.started += _ => cameraWork.SwitchPlayer();

    }

    // Start is called before the first frame update
    void Start() {
        pc = GetComponent<PlayerController>();
        pv = GetComponent<PhotonView>();
        cameraWork = GetComponent<CameraWork>();
        cameraAnimator = Camera.main.gameObject.GetComponent<Animator>();
        
    }

    public void StartCameraCutscene(float length) {
        cameraCutscene = true;
        if (pc != null) {
            pc.OnMovement(Vector2.zero);
            pc.AttackOne(false);
            pc.AttackAlt(false);
        }
        if (length < 0) {
            length = cameraCutsceneLength;
        }
        Debug.Log("camera custene start");
        Invoke("StopCameraCutscene", length);
    }

    public void StopCameraCutscene() {
        cameraCutscene = false;
        Debug.Log("stop camerda");
    }

    
    void Reload() {
        pc.Reload();
    }

    void SwitchWeapon() {
        pc.SwitchWeapon();
    }

    public void EnableInput() {
        inputEnabled = true;
    }

    public void DisableInput() {
        inputEnabled = false;
        Vector2 newMovementInput = new Vector2(0,0);
        pc.OnMovement(newMovementInput);
    }

    private void OnEnable() {
        inputController.Enable();
    }

    private bool CanMove() {
        return !cameraCutscene && inputEnabled;
    }

    private void OnDisable() {
        inputController.Disable();
        pc.OnMovement(Vector2.zero);
    }

    void AttackOne(InputAction.CallbackContext ctx) {
        if (CanMove()) {
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
        if (CanMove()) {
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
        if (CanMove()) {
            pc.ChangeLight();
        }
    }

    void ChangePerspective() {
        if (CanMove()) {
            if (cameraAnimator == null) {
                cameraAnimator = Camera.main.gameObject.GetComponent<Animator>();
            }
            cameraAnimator.SetTrigger("changePersp");
        }
    }

    void Dash(InputAction.CallbackContext ctx) {
        if (CanMove()) {
            pc.Dash();
        }
    }

    public void OnMovement(InputAction.CallbackContext ctx) {
        if (CanMove()) {
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

    public void MicOutputRecieved() {
        micRequestActive = false;
    }
    public void VoiceControl(InputAction.CallbackContext ctx) {
        if (pv.IsMine) {
            if (CanMove()) {
                if(GlobalValues.Instance.micEnabled && GlobalValues.Instance.voiceControlEnabled) {
                    if (!micRequestActive) {
                        micRequestActive = true;
                        micRenderer.enabled = true;
                        startRecogniser();
                    } 
                }
                else {
                    micRenderer.enabled = false;
                    ChangeLight();
                }
                    
            }
        }
    }
    public void ToggleHelpTooltip(InputAction.CallbackContext ctx) {
        if (pv.IsMine) {
            if (helpView == null) {
                GameObject controlsHelp = GlobalValues.Instance.UIElements.transform.Find("ControlsPNG").gameObject;
                HelpTooltip actualScript = controlsHelp.GetComponent<HelpTooltip>();
                helpView = actualScript;
            }
            helpView.ToggleVisibility();
        }
    }
    public void ToggleMenu(InputAction.CallbackContext ctx) {
        if (gameObject == PlayerController.LocalPlayerInstance) {
            if (menuView == null)
                menuView = GlobalValues.Instance.MenuItem.GetComponent<InGameMenu>();
            inputEnabled = !menuView.ToggleVisibility();
            if (!inputEnabled) {
                pc.AttackOne(false);
                pc.AttackAlt(false);
                pc.OnMovement(Vector2.zero);
            }
        }
        
    }
}


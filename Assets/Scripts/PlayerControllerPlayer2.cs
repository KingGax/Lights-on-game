using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerPlayer2 : MonoBehaviour
{
    Rigidbody rb;
    public Vector3 playerSpeed;
    private PlayerInputsPlayer2 inputController;
    private PlayerInputsPlayer2.PlayerActions movementInputMap;
    public float moveSpeed;
    Vector2 movement;
    Vector3 cameraForward;
    Vector3 cameraRight;
    Vector3 XZPlaneNormal = new Vector3(0, 1, 0);
    public Camera cam;
    public Light lantern;
    Color lightColour;
    Color[] colours = { new Color(1, 0, 0), new Color(0, 1, 0), new Color(0, 0, 1) };
    int colourIndex = 0;

    LightObject lo;

    // Start is called before the first frame update

    void Awake()
    {
        inputController = new PlayerInputsPlayer2();

        lo = GetComponentInChildren<LightObject>();

        movementInputMap = inputController.Player;

        movementInputMap.Movement.performed += ctx => OnMovement(ctx);
        movementInputMap.Movement.started += ctx => OnMovement(ctx);
        movementInputMap.Movement.canceled += ctx => OnMovement(ctx);

        movementInputMap.Light.started += _ => ChangeLight();

    }
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        cameraForward = Vector3.ProjectOnPlane(cam.transform.forward, XZPlaneNormal);
        cameraRight = Vector3.ProjectOnPlane(cam.transform.right, XZPlaneNormal);
        lantern.color = colours[colourIndex];
    }


    // Update is called once per frame
    void Update()
    {
        
        rb.velocity = cameraForward * movement.y * moveSpeed + cameraRight * movement.x * moveSpeed;
    }

    void ChangeLight()
    {
        colourIndex = (colourIndex + 1) % 3;
        lantern.color = colours[colourIndex];
        lo.colour = colours[colourIndex];
        lo.ChangeColour();
        
    }

    public void OnMovement(InputAction.CallbackContext ctx)
    {
        Vector2 newMovementInput = ctx.ReadValue<Vector2>();
        movement = newMovementInput;
    }

    private void OnEnable()
    {
        inputController.Enable();
    }
    private void OnDisable()
    {
        inputController.Disable();
    }
}

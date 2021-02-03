using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    Rigidbody rb;
    public Vector3 playerSpeed;
    private PlayerInputs inputController;
    private PlayerInputs.PlayerActions movementInputMap;
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
    Plane playerPlane;
    public IGun weaponScript;

    public LightObject lo;
    bool fireHeld = false;

    // Start is called before the first frame update

    void Awake()
    {
        inputController = new PlayerInputs();

        lo = GetComponentInChildren<LightObject>();

        movementInputMap = inputController.Player;

        movementInputMap.Movement.performed += ctx => OnMovement(ctx);
        movementInputMap.Movement.started += ctx => OnMovement(ctx);
        movementInputMap.Movement.canceled += ctx => OnMovement(ctx);

        movementInputMap.Light.started += _ => ChangeLight();

        movementInputMap.Attack.started += ctx => AttackOne(ctx);
        movementInputMap.Attack.performed += ctx => AttackOne(ctx);
    }
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        cameraForward = Vector3.ProjectOnPlane(cam.transform.forward, XZPlaneNormal);
        cameraRight = Vector3.ProjectOnPlane(cam.transform.right, XZPlaneNormal);
        lantern.color = colours[colourIndex];
        playerPlane = new Plane(XZPlaneNormal, transform.position);
        cam = Camera.main;
    }


    // Update is called once per frame
    void Update()
    {
        if (fireHeld)
        {
            transform.LookAt(GetFireDirection() + transform.position);
        }
        rb.velocity = cameraForward * movement.y * moveSpeed + cameraRight * movement.x * moveSpeed;
    }

    Vector3 GetFireDirection()
    {
        //Create a ray from the Mouse click position
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        //Initialise the enter variable
        float enter = 0.0f;

        if (playerPlane.Raycast(ray, out enter))
        {
            //Get the point that is clicked
            Vector3 hitPoint = ray.GetPoint(enter);
            Vector3 fireDirection = Vector3.ProjectOnPlane(hitPoint - transform.position, XZPlaneNormal);
            return fireDirection;
        }
        else return Vector3.zero;
    }

    void AttackOne(InputAction.CallbackContext ctx)
    {

        if (ctx.performed)
        {//performed in this case means released
            fireHeld = false;
        }
        else
        {
            fireHeld = true;
            transform.LookAt(GetFireDirection() + transform.position);
            weaponScript.Shoot(GetFireDirection());
        }

        
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    

    public float turnSpeed;
    public float moveSpeed;

    public Camera cam;
    public Light lantern;
    public IGun weaponScript;
    public LightObject lo;

    private PlayerInputs inputController;
    private PlayerInputs.PlayerActions movementInputMap;
    Rigidbody rb;
    Vector2 movement;
    Vector3 cameraForward;
    Vector3 cameraRight;
    Vector3 XZPlaneNormal = new Vector3(0, 1, 0);
   
    Color lightColour;
    Color[] colours = { new Color(1, 0, 0), new Color(0, 1, 0), new Color(0, 0, 1) };
    int colourIndex = 0;
    Plane playerPlane;

    [Header("Shooting")]
    public float shootBufferMax;
    public LayerMask aimTargetsMask;
    public float maxShootOffsetAngle;
    float shootBuffer;
    bool fireHeld = false;


    [Header("Dashing")]
    public float dashSpeed;
    public float dashDurationTimerMax;
    public float dashCooldownMax;
    public float dashBufferMax;
    float dashDurationTimer = 0;
    float dashCooldown = 0;
    float dashBuffer = 0;
    bool dashing = false;
    Vector3 dashDirection;
    bool canDash = true;

    // Start is called before the first frame update

    void Awake()
    {
        inputController = new PlayerInputs();

        lo = GetComponentInChildren<LightObject>();

        movementInputMap = inputController.Player;

        movementInputMap.Movement.performed += ctx => OnMovement(ctx);
        movementInputMap.Movement.started += ctx => OnMovement(ctx);
        movementInputMap.Movement.canceled += ctx => OnMovement(ctx);
        movementInputMap.Dash.started += ctx => Dash(ctx);
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
        cam = Camera.main;
        StartCoroutine("CountdownTimers");
    }

    bool CanShoot(){
        return !dashing;
    }
    // Update is called once per frame
    void Update()
    {
        playerPlane = new Plane(XZPlaneNormal, transform.position); // small optimisation can be made by moving this to start and making sure player y is right at the start
        if (dashBuffer > 0){
            if (!dashing && canDash){
                StartDash();
            }
        }
        if (fireHeld)
        {
            shootBuffer = shootBufferMax;
        }
        if (shootBuffer > 0 && CanShoot())
        {
            Vector3 fireDirection = GetFireDirection();
            transform.forward = Vector3.RotateTowards(transform.forward, fireDirection , Time.deltaTime * turnSpeed, 0.0f);
            if (Vector3.Angle(transform.forward, fireDirection) <= maxShootOffsetAngle)
            {
                bool didShoot = weaponScript.RequestShoot(GetFireDirection());
            }
        }
        if (dashing)
        {  
            HandleDash();
        }
        else
        {
            Vector3 moveVector = cameraForward * movement.y * moveSpeed + cameraRight * movement.x * moveSpeed;
            moveVector.y = rb.velocity.y;
            rb.velocity = moveVector;
        }
    }

    void StartDash()
    {
        dashing = true;
        canDash = false;
        dashDurationTimer = dashDurationTimerMax;
        if (movement == Vector2.zero)
        {
            dashDirection = transform.forward;
        }
        else
        {
            dashDirection = cameraForward* movement.y + cameraRight * movement.x;
        }
    }
    void HandleDash(){
        rb.velocity = dashDirection * dashSpeed;
    }

    Vector3 GetFireDirection()
    {
        //Create a ray from the Mouse click position
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        //Initialise the enter variable
        float enter = 0.0f;

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f, aimTargetsMask))
        {
            Vector3 hitPoint = playerPlane.ClosestPointOnPlane(hit.point);
            Vector3 fireDirection = Vector3.ProjectOnPlane(hit.point - transform.position, XZPlaneNormal);
            return fireDirection;
        }
        else if (playerPlane.Raycast(ray, out enter))
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
            
        }
    }

    void Dash(InputAction.CallbackContext ctx){
        dashBuffer = dashBufferMax;
        
        //Vector3 movementVector = new Vector3(movement.x, 0, movement.y);
        //rb.AddForce(dashForce*transform.forward);
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

        // Vector3 directionVector = transform.position - new Vector3(newMovementInput.x, 0, newMovementInput.y);
        // transform.LookAt(directionVector);
        //if (ctx.)
        //Vector3 forwardVector = 
        if (movement != Vector2.zero){
            transform.forward = cameraForward * movement.y  + cameraRight * movement.x;
        }
         
        //transform.Rotate(new Vector3(0, 30, 0), Space.World);
    }

    private void OnEnable()
    {
        inputController.Enable();
    }
    
    private void OnDisable()
    {
        inputController.Disable();
    }

    private IEnumerator CountdownTimers()
    {
        while (true)
        {
            if (dashDurationTimer > 0)
            {
                dashDurationTimer -= Time.deltaTime;
                if (dashDurationTimer <= 0)
                {
                    dashing = false;  
                    dashCooldown = dashCooldownMax;
                }
            }
            if (dashCooldown > 0)
            {
                dashCooldown-=Time.deltaTime;
                if (dashCooldown <=0){
                    canDash = true;
                }
            }
            if (dashBuffer > 0){
                dashBuffer -= Time.deltaTime;
            }
            if (shootBuffer > 0)
            {
                shootBuffer -= Time.deltaTime;
            }
            yield return null;
        }
    }
}

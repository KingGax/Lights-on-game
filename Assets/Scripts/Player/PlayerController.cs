using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

public enum LanternColour {
    Red,
    Green,
    Blue,
}
public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable, IKnockbackable {
    
    public float turnSpeed;
    public float moveSpeed;

    [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
    public static GameObject LocalPlayerInstance;

    Camera cam;
    public Light lantern;
    public Weapon equiptedWeapon;
    public LightObject lightSource;

    Rigidbody rb;
    Vector2 movement;
    Vector3 cameraForward;
    Vector3 cameraRight;
    Vector3 XZPlaneNormal = new Vector3(0, 1, 0);
    public GameObject UIElements;

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
    public bool isTakingKnockback { get; set; }


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

    #region IPunObservable implementation
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.IsWriting) {
        // We own this player: send the others our data
            
        } else {
           
        }
    }
    #endregion

    void Awake() {
        lightSource = GetComponentInChildren<LightObject>();
        cam = Camera.main;
        
        // #Important
        // used in GameManager.cs: we keep track of the localPlayer instance to prevent instantiation when levels are synchronized
        if (photonView.IsMine) {
            PlayerController.LocalPlayerInstance = this.gameObject;
            Debug.Log("Henlo");
            GameObject UI = Instantiate(UIElements);
            DontDestroyOnLoad(UI);
            //cam.GetComponent<CameraController>().bindToPlayer(this.gameObject.transform);
        }
        // #Critical
        // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
        DontDestroyOnLoad(this.gameObject);
        
            
    }

    void Start() {
        CameraWork _cameraWork = this.gameObject.GetComponent<CameraWork>();
        GlobalValues.Instance.AddPlayer(gameObject);
        if (_cameraWork != null) {
            if (photonView.IsMine) {
                _cameraWork.OnStartFollowing();
                GlobalValues.Instance.localPlayerInstance = this.gameObject;
            }
        } else {
            Debug.LogError("<Color=Red><a>Missing</a></Color> CameraWork Component on playerPrefab.", this);
        }

        rb = gameObject.GetComponent<Rigidbody>();
        cameraForward = Vector3.ProjectOnPlane(cam.transform.forward, XZPlaneNormal);
        cameraRight = Vector3.ProjectOnPlane(cam.transform.right, XZPlaneNormal);
        lantern.color = colours[colourIndex];
        
        StartCoroutine("CountdownTimers");
    }

    [PunRPC]
    void UpdateLightColour(int newIndex) {
        colourIndex = newIndex;
        lantern.color = colours[colourIndex];
        lightSource.colour = colours[colourIndex];
        lightSource.ChangeColour();
    }

    bool CanShoot() {
        return !dashing;
    }

    void TurnTowards(Vector3 direction) {
        transform.forward = Vector3.RotateTowards(transform.forward, direction, Time.deltaTime * turnSpeed, 0.0f);
    }

    // Update is called once per frame
    void Update() {
        if (fireHeld) {
            shootBuffer = shootBufferMax;
        }
        playerPlane = new Plane(XZPlaneNormal, transform.position); // small optimisation can be made by moving this to start and making sure player y is right at the start

        if (dashBuffer > 0) {
            if (!dashing && canDash) {
                StartDash();
            }
        }
        
        //handles looking and shooting
        if (shootBuffer > 0 && CanShoot()) {
            Vector3 fireDirection = GetFireDirection();
            TurnTowards(fireDirection);
            if (Vector3.Angle(transform.forward, fireDirection) <= maxShootOffsetAngle) {
                bool didShoop = equiptedWeapon.Use();
            }
        } else {
            if (movement != Vector2.zero) {
                if (photonView.IsMine == true || PhotonNetwork.IsConnected == false) {
                    Vector3 moveVector = cameraForward * movement.y * moveSpeed + cameraRight * movement.x * moveSpeed;
                    TurnTowards(moveVector);
                }
                
            }
        }
        if (!isTakingKnockback)
        {
            if (dashing)
            {
                HandleDash();
            }
            else
            {
                if (photonView.IsMine == true || PhotonNetwork.IsConnected == false)
                {
                    Vector3 moveVector = cameraForward * movement.y * moveSpeed + cameraRight * movement.x * moveSpeed;
                    moveVector.y = rb.velocity.y;
                    rb.velocity = moveVector;
                }
            }
        }
    }

    void StartDash() {
        dashing = true;
        canDash = false;
        dashDurationTimer = dashDurationTimerMax;
        if (movement == Vector2.zero) {
            dashDirection = transform.forward;
        } else {
            dashDirection = cameraForward* movement.y + cameraRight * movement.x;
        }
    }

    void HandleDash() {
        rb.velocity = dashDirection * dashSpeed;
    }

    Vector3 GetFireDirection() {
        //Create a ray from the Mouse click position
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        //Initialise the enter variable
        float enter = 0.0f;

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f, aimTargetsMask)) {
            Vector3 hitPoint = playerPlane.ClosestPointOnPlane(hit.point);
            Vector3 fireDirection = Vector3.ProjectOnPlane(hit.point - transform.position, XZPlaneNormal);
            return fireDirection;
        } else if (playerPlane.Raycast(ray, out enter)) {
            //Get the point that is clicked
            Vector3 hitPoint = ray.GetPoint(enter);
            Vector3 fireDirection = Vector3.ProjectOnPlane(hitPoint - transform.position, XZPlaneNormal);
            return fireDirection;
        } else return Vector3.zero;
    }

    public void AttackOne(bool mouseDown) {
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true) {
            return;
        }
        fireHeld = mouseDown;
    }

    public void Dash() {
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
        {
            return;
        }
        dashBuffer = dashBufferMax;
    }

    public void ChangeLight() {
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true) {
            return;
        }
        colourIndex = (colourIndex + 1) % 3;
        lantern.color = colours[colourIndex];
        lightSource.colour = colours[colourIndex];
        photonView.RPC("UpdateLightColour", RpcTarget.OthersBuffered, colourIndex);
        lightSource.ChangeColour();
    }

    public void TakeKnockback(Vector3 dir, float magnitude, float duration) {
        if (!isTakingKnockback) {
            isTakingKnockback = true;
            rb.velocity = dir * magnitude;
            Invoke("EndKnockback", duration);
        }
    }

    void EndKnockback() {
        isTakingKnockback = false;
    }


    public void ChangeLightToColour(LanternColour col) {
        switch (col) {
            case LanternColour.Red:
                colourIndex = 0;
                break;
            case LanternColour.Green:
                colourIndex = 1;
                break;
            case LanternColour.Blue:
                colourIndex = 2;
                break;
            default:
                break;
        }

        lantern.color = colours[colourIndex];
        lightSource.colour = colours[colourIndex];
        lightSource.ChangeColour();
    }

    public void OnMovement(Vector2 newMovementInput) {
        movement = newMovementInput;
    }

    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.layer != GlobalValues.Instance.environment && isTakingKnockback) {
            Debug.Log("Collided with environment");
            EndKnockback();
        }
    }

    private IEnumerator CountdownTimers() {
        while (true) {
            if (dashDurationTimer > 0) {
                dashDurationTimer -= Time.deltaTime;
                if (dashDurationTimer <= 0) {
                    dashing = false;
                    dashCooldown = dashCooldownMax;
                }
            }

            if (dashCooldown > 0) {
                dashCooldown-=Time.deltaTime;
                if (dashCooldown <=0) {
                    canDash = true;
                }
            }

            if (dashBuffer > 0) {
                dashBuffer -= Time.deltaTime;
            }

            if (shootBuffer > 0) {
                shootBuffer -= Time.deltaTime;
            }

            yield return null;
        }
    }
}
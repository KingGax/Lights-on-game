using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;
using Photon.Realtime;
using LightsOn.AudioSystem;
using LightsOn.HealthSystem;
using LightsOn.LightingSystem;
using LightsOn.WeaponSystem;

public class PlayerController : MonoBehaviourPunCallbacks, IKnockbackable, IOnPhotonViewOwnerChange {

    public float turnSpeed;
    public float moveSpeed;

    [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
    public static GameObject LocalPlayerInstance;

    Camera cam;
    public SpriteRenderer micRenderer;
    public PlayerWeapon equiptedWeapon;
    public List<PlayerWeapon> weapons;

    public Lanturn lanturn;

    public MeshRenderer playerRenderer;
    int weaponIndex = 0;
    Rigidbody rb;
    Vector2 movement;
    Vector3 cameraForward;
    Vector3 cameraRight;
    Vector3 XZPlaneNormal = new Vector3(0, 1, 0);
    public GameObject UIElements;

    Plane playerPlane;

    [Header("Shooting")]
    public float shootBufferMax; //input buffer for shooting
    public float altShootBufferMax;
    public float reloadSpeedModifier;
    public float reloadBufferMax; //input buffer for reload
    public LayerMask aimTargetsMask;
    public float maxShootOffsetAngle;
    float shootBuffer = 0;
    float altShootBuffer = 0;
    bool altFireReleasedThisFrame = false;
    bool fireHeld = false;
    bool altFireHeld = false;
    float currentChargeSpeedModifier; //movement modifier for equipped weapon
    float reloadBuffer;
    bool reloading;
    private int hiddenLayer;
    private int defaultLayer;
    private PlayerInputScript inputScript;
    public bool isTakingKnockback { get; set; }
    public MeshRenderer gunRenderer;

    [Header("Dashing")]
    public TrailRenderer dashTrail;
    public ParticleSystem dashParticles;
    public float dashSpeed;
    public float dashDurationTimerMax; //dash length
    public float dashCooldownMax; //dash cooldown
    public float dashBufferMax; //dash input buffer
    public float dashVulnerability; //time invisible during dash
    float dashDurationTimer = 0;
    float dashCooldown = 0;
    float dashBuffer = 0;
    bool dashing = false;
    Vector3 dashDirection;
    bool canDash = true;
    bool hidden = false;
    bool movementEnabled = true;
    bool spectator = false;
    bool initialised = false;
    public Animator anim;
    SkinnedMeshRenderer[] shawnRenderers;
    public GameObject shawn; //player model refrence
    public Transform firePoint;
    private float altFireCooldownTimer = 0;
    public float altFireCooldownTimerMax;

    //Callback to handle the object being passed to a rejoining player
    void IOnPhotonViewOwnerChange.OnOwnerChange(Player newOwner, Player oldOwner) {
        if (PhotonNetwork.LocalPlayer == newOwner) {
            UpdateLocalPlayerInstance();
        }
    }

    void Awake() {
        photonView.AddCallbackTarget(this);
        lanturn = GetComponentInChildren<Lanturn>();
        rb = gameObject.GetComponent<Rigidbody>();
        cam = Camera.main;
        defaultLayer = gameObject.layer;
        inputScript = GetComponent<PlayerInputScript>();
        hiddenLayer = LayerMask.NameToLayer("HiddenPlayer");
        // #Important
        // used in GameManager.cs: we keep track of the localPlayer instance to prevent instantiation when levels are synchronized
        if (photonView.IsMine) {
            PlayerController.LocalPlayerInstance = this.gameObject;
            FloatingHealthBar fhb = gameObject.GetComponentInChildren<FloatingHealthBar>();
            fhb.enabled = false;
            fhb.gameObject.GetComponent<Canvas>().enabled = false;
            //cam.GetComponent<CameraController>().bindToPlayer(this.gameObject.transform);
        } else {

        }
        // #Critical
        // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
        DontDestroyOnLoad(this.gameObject);
        initialised = true;
        shawnRenderers = shawn.GetComponentsInChildren<SkinnedMeshRenderer>();
    }

    //Function to handle the object being passed to a rejoining player
    void UpdateLocalPlayerInstance() {
        if (initialised) {
            if (photonView.IsMine) {
                PlayerController.LocalPlayerInstance = this.gameObject;
                FloatingHealthBar fhb = gameObject.GetComponentInChildren<FloatingHealthBar>();
                fhb.gameObject.GetComponent<Canvas>().enabled = false;
                fhb.enabled = false;

                GlobalValues.Instance.navManager.SetPlayer(GlobalValues.Instance.players[0] != GlobalValues.Instance.localPlayerInstance);
                CameraWork _cameraWork = this.gameObject.GetComponent<CameraWork>();
                if (_cameraWork != null) {
                    if (photonView.IsMine) {
                        gameObject.name = "LocalPlayer";
                        _cameraWork.OnStartFollowing();//setup camera
                        GlobalValues.Instance.localPlayerInstance = this.gameObject;
                        PlayerHealth h = GetComponent<PlayerHealth>();
                        h.Start();
                        rb.isKinematic = false;//allow movement
                    } else {

                    }
                }
                //cam.GetComponent<CameraController>().bindToPlayer(this.gameObject.transform);
            } else { //waiting if ownership has not been registered yet
                rb.isKinematic = true;
                Invoke("UpdateLocalPlayerInstance", 0.5f);
            }
        } else {
            Invoke("UpdateLocalPlayerInstance", 0.5f);
        }

    }

    //RPC function to update changed weapons across the network
    [PunRPC]
    public void SetWeaponRPC(int wepIndex) {
        foreach (Weapon wep in weapons) {
            wep.UnequipWeapon();
        }
        weapons[wepIndex].EquipWeapon();
        equiptedWeapon = weapons[wepIndex];
        currentChargeSpeedModifier = equiptedWeapon.chargeSpeedModifier;
    }

    public void SwitchWeapon() {
        weaponIndex = (weaponIndex + 1) % weapons.Count;
        photonView.RPC("SetWeaponRPC", RpcTarget.All, weaponIndex);
    }

    void Start() {
        CameraWork _cameraWork = this.gameObject.GetComponent<CameraWork>();
        GlobalValues.Instance.AddPlayer(gameObject);
        int index = GlobalValues.Instance.players.IndexOf(gameObject);
        Room room = PhotonNetwork.CurrentRoom;
        int playerCount;
        try {
            playerCount = (int)room.CustomProperties["playerCount"];
        } catch (System.Exception) {
            playerCount = 1;
            Debug.LogError("no player count room property found");
        }
        if (index >= playerCount) {
            spectator = true;
            _cameraWork.TargetPlayer(0);
        }
        if (_cameraWork != null) {
            if (photonView.IsMine) {
                gameObject.name = "LocalPlayer";
                _cameraWork.OnStartFollowing();
                GlobalValues.Instance.localPlayerInstance = this.gameObject;
            } else {
                rb.isKinematic = true;
            }
        } else {
            Debug.LogError("<Color=Red><a>Missing</a></Color> CameraWork Component on playerPrefab.", this);
        }
        cameraForward = Vector3.Normalize(Vector3.ProjectOnPlane(cam.transform.forward, XZPlaneNormal));
        cameraRight = Vector3.Normalize(Vector3.ProjectOnPlane(cam.transform.right, XZPlaneNormal));

        foreach (Weapon wep in weapons) {
            wep.UnequipWeapon();
        }
        weapons[weaponIndex].EquipWeapon();
        currentChargeSpeedModifier = equiptedWeapon.chargeSpeedModifier;

        StartCoroutine("CountdownTimers");
    }

    //function for disabling/enabling movement
    public void SetMovementEnabled(bool enabled) {
        movementEnabled = enabled;
        if (!enabled) {
            rb.velocity = Vector3.zero;
        }
    }

    bool CanShoot() {
        return !dashing && !reloading;
    }

    void TurnTowards(Vector3 direction) {
        transform.forward = Vector3.RotateTowards(transform.forward, direction, Time.deltaTime * turnSpeed, 0.0f);
    }

    void Update() {
        if (dashTrail.time > 0) {//handles keeping dash trail updated
            dashTrail.time -= Time.deltaTime;
        }
        if (photonView == null || !photonView.IsMine) return;



        if (movementEnabled) {
            reloading = equiptedWeapon.IsReloading();
            //updates shooting buffer
            if (fireHeld) {
                shootBuffer = shootBufferMax;
            }
            if (altFireHeld) {
                altShootBuffer = altShootBufferMax;
            }
            playerPlane = new Plane(XZPlaneNormal, firePoint.position); // small optimisation can be made by moving this to start and making sure player y is right at the start

            if (dashBuffer > 0 && !dashing && canDash) {
                StartDash();
            }

            if (hidden && !dashing) {
                ShowPlayer();
            }

            //handles looking and shooting
            if (equiptedWeapon.IsCharging()) {//handles charging attack
                anim.SetBool("ChargingAlt", true);
                if (altFireReleasedThisFrame) { //handles releasing charged attack
                    altFireReleasedThisFrame = false;
                    if (equiptedWeapon.ReleaseWeaponAlt()) {
                        anim.SetTrigger("FireAlt");
                        altFireCooldownTimer = altFireCooldownTimerMax;
                    }
                } else if (altFireHeld && CanShoot()) {//starts charging alt fire
                    Vector3 fireDirection = GetFireDirection(false);
                    TurnTowards(fireDirection);
                    equiptedWeapon.UseAlt();
                }
            } else if (shootBuffer > 0 && CanShoot()) {//handles default shooting
                Vector3 fireDirection = GetFireDirection(true);
                TurnTowards(fireDirection);
                anim.SetBool("Shooting", true);
                anim.SetBool("ChargingAlt", false);
                if (Vector3.Angle(transform.forward, fireDirection) <= maxShootOffsetAngle) {
                    bool didShoop = equiptedWeapon.Use();
                }
            } else if (altFireHeld && CanShoot()) {
                anim.SetBool("ChargingAlt", true);//this is the first frame charging starts, only called once 
                Vector3 fireDirection = GetFireDirection(false);
                TurnTowards(fireDirection);
                equiptedWeapon.UseAlt();
            } else if (reloading && shootBuffer > 0 || altFireHeld) { //this is for looking somewhere but not actually shooting
                anim.SetBool("ChargingAlt", false);
                Vector3 fireDirection = GetFireDirection(altFireHeld);
                TurnTowards(fireDirection);
            } else { //not firing currently
                anim.SetBool("Shooting", false);
                anim.SetBool("ChargingAlt", false);
                if (altFireCooldownTimer > 0) {//lock to where you just shot so that it does not instantly turn away

                } else if (movement != Vector2.zero) {//look where you are moving
                    if (photonView.IsMine == true || PhotonNetwork.IsConnected == false) {
                        Vector3 moveVector = cameraForward * movement.y * moveSpeed + cameraRight * movement.x * moveSpeed;
                        TurnTowards(moveVector);
                    }

                }
            }
            if (!isTakingKnockback) {//this handles player movement
                if (dashing) {//dash is highest priority
                    HandleDash();
                } else {//if not dashing move normally
                    if (reloadBuffer > 0 && CanShoot()) {//this is here as you can only reload if not shooting, dashing or being knocked back
                        equiptedWeapon.Reload();
                    }

                    if (photonView.IsMine == true || PhotonNetwork.IsConnected == false) {
                        Vector3 moveVector = cameraForward * movement.y * moveSpeed + cameraRight * movement.x * moveSpeed;
                        if (equiptedWeapon.IsCharging()) {
                            moveVector *= currentChargeSpeedModifier;
                        }
                        moveVector.y = rb.velocity.y;
                        rb.velocity = moveVector;//animation blending parameters
                        float velocityZ = Vector3.Dot(moveVector.normalized, transform.forward);
                        float velocityX = Vector3.Dot(moveVector.normalized, transform.right);
                        anim.SetFloat("VelocityX", velocityX, 0.1f, Time.deltaTime);
                        anim.SetFloat("VelocityZ", velocityZ, 0.1f, Time.deltaTime);
                        if (moveVector.magnitude > 0.1) {
                            anim.SetBool("Moving", true);
                        } else {
                            anim.SetBool("Moving", false);
                        }
                    }
                }
            }
        } else {
            if (!isTakingKnockback) {
                rb.velocity = new Vector3(0, rb.velocity.y, 0); //movement disabled and not knocked back, so do not move
            }
        }
    }

    //Function for initiating dashes
    void StartDash() {
        dashing = true;
        canDash = false;
        HidePlayer();
        AudioManager.Instance.PlaySFX(SoundClips.Instance.SFXDash, transform.position, gameObject);
        dashDurationTimer = dashDurationTimerMax;
        if (movement == Vector2.zero) { //dashes where currently looking otherwise dashes where moving
            dashDirection = transform.forward;
        } else {
            dashDirection = cameraForward * movement.y + cameraRight * movement.x;
        }
    }

    public void Die() {
        anim.SetBool("ChargingAlt", false);
        anim.SetBool("Shooting", false);
        anim.SetTrigger("Death");
    }

    //RPC for making player invisible on dash
    [PunRPC]
    void HidePlayerRPC() {
        gameObject.layer = hiddenLayer;
        dashParticles.Play();
        dashTrail.time = 1.0f;
        foreach (SkinnedMeshRenderer renderer in shawnRenderers) {
            renderer.enabled = false;
        }
        hidden = true;
    }
    void HidePlayer() {
        photonView.RPC("HidePlayerRPC", RpcTarget.All);
    }

    //RPC for making player visible after dash
    [PunRPC]
    void ShowPlayerRPC() {
        gameObject.layer = defaultLayer;
        foreach (SkinnedMeshRenderer renderer in shawnRenderers) {
            renderer.enabled = true;
        }
        hidden = false;
    }

    void ShowPlayer() {
        photonView.RPC("ShowPlayerRPC", RpcTarget.All);
    }

    void HandleDash() {
        if (!hidden && dashDurationTimer < dashVulnerability) {
            ShowPlayer();
        }
        rb.velocity = dashDirection * dashSpeed;
    }

    //Function to get the direction the player clicks, can optionally lock aim onto enemies
    Vector3 GetFireDirection(bool lockToEnemies) {
        //Create a ray from the Mouse click position
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        //Initialise the enter variable
        float enter = 0.0f;

        //Either gets point by raycasting into scene or by raycasting onto player plane
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f, aimTargetsMask) && lockToEnemies) {
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

    public void AttackAlt(bool mouseDown) {
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true) {
            return;
        }
        if (!mouseDown) {
            altFireReleasedThisFrame = true;
        }
        altFireHeld = mouseDown;
    }

    public void Dash() {
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true) {
            return;
        }
        dashBuffer = dashBufferMax;
    }

    public void ChangeLight() {
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true) {
            return;
        }
        LightColour c = (LightColour)((int)lanturn.GetColour() >> 8);
        if (c == 0) c = LightColour.Red;
        lanturn.SetColour(c);
        AudioManager.Instance.PlaySFX(SoundClips.Instance.SFXLightChange, transform.position, gameObject);
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

    public void ChangeLightToColourText(string colour) {
        micRenderer.enabled = false;
        inputScript.MicOutputRecieved();
        if (colour == "GREEN") {
            lanturn.SetColour(LightColour.Green);
        } else if (colour == "RED") {
            lanturn.SetColour(LightColour.Red);
        } else if (colour == "BLUE") {
            lanturn.SetColour(LightColour.Blue);
        }
    }

    public void OnMovement(Vector2 newMovementInput) {
        movement = newMovementInput;
    }

    public void Reload() {
        reloadBuffer = reloadBufferMax;
    }

    //Unity collision clalback used to cancel knockback
    private void OnCollisionEnter(Collision other) {
        if ((GlobalValues.Instance.environment | (1 << other.gameObject.layer)) == GlobalValues.Instance.environment && isTakingKnockback) {
            EndKnockback();
        }
    }

    //Coroutine that counts down all timers
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
                dashCooldown -= Time.deltaTime;
                if (dashCooldown <= 0) {
                    canDash = true;
                }
            }

            if (dashBuffer > 0) {
                dashBuffer -= Time.deltaTime;
            }
            if (shootBuffer > 0) {
                shootBuffer -= Time.deltaTime;
            }
            if (altShootBuffer > 0) {
                altShootBuffer -= Time.deltaTime;
            }
            if (reloadBuffer > 0) {
                reloadBuffer -= Time.deltaTime;
            }
            if (altFireCooldownTimer > 0) {
                altFireCooldownTimer -= Time.deltaTime;
            }

            yield return null;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TutorialHelper : MonoBehaviour
{
    private PlayerInputs inputController;
    private PlayerInputs.PlayerActions movementInputMap;
    private GameObject player;
    public Transform tutorialBotEnemyTransform;

    public List<string> tooltipMessages;
    private int currentTooltipIndex = -1;
    private Tooltip tip;
    bool playerFound = false;
    public Vector3 offset;
    private bool performedShoot = false;
    private bool performedMove = false;
    private bool performedDash = false;
    private bool performedLight = false;
    private bool performedRightClick = false;
    public float tooltipChangeDelay;
    private bool delayingChange = false;
    private bool trackingPlayer1 = true;
    private bool finished = false;
    

    void Awake() {
        inputController = new PlayerInputs();

        movementInputMap = inputController.Player;

        movementInputMap.Movement.performed += ctx => {  performedMove = true; };
        movementInputMap.Movement.started += ctx => { performedMove = true; };
        movementInputMap.Movement.canceled += ctx => { performedMove = true; };
        movementInputMap.Dash.started += ctx => {  performedDash = true; };
        movementInputMap.Light.started += ctx => {  performedLight = true; };



        movementInputMap.Attack.started += ctx => {  performedShoot = true; };
        movementInputMap.Attack.performed += ctx => {  performedShoot = true; };
        movementInputMap.AltAttack.started += ctx => { performedRightClick = true; };
        movementInputMap.AltAttack.performed += ctx => { performedRightClick = true; };

    }

    private void Start() {
        tip = gameObject.GetComponentInChildren<Tooltip>();
    }

    private void OnEnable() {
        inputController.Enable();
    }

    private void OnDisable() {
        inputController.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerFound) {
            if (GlobalValues.Instance.localPlayerInstance != null) {
                player = GlobalValues.Instance.localPlayerInstance;
                playerFound = true;
                if (player == GlobalValues.Instance.players[0]) {
                    trackingPlayer1 = true;
                }
                else {
                    trackingPlayer1 = false;
                }
                NextEvent();
            }
        }
        else {
            if (!finished) {
                transform.position = player.transform.position + offset;
                if (!delayingChange) {
                    CheckEventInput();
                }
            }
        }
        
    }
    void NextEvent() {
        currentTooltipIndex++;
        switch (currentTooltipIndex) {
            case 0:
                performedMove = false;
                break;
            case 1:
                performedDash = false;
                break;
            case 2:
                performedLight = false;
                break;
            case 4:
                performedShoot = false;
                break;
            case 5:
                performedRightClick = false;
                break;
            default:
                break;
        }
        if (currentTooltipIndex < tooltipMessages.Count) {
            tip.SetTextLocal(tooltipMessages[currentTooltipIndex]);
        }
        else {
            tip.gameObject.SetActive(false);
            tip.enabled = false;
            finished = true;
        }
        delayingChange = false;

    }
    void CheckEventInput() {
        switch (currentTooltipIndex) {
            case 0:
                if (performedMove) {
                    Invoke("NextEvent",tooltipChangeDelay);
                    delayingChange = true;
                }
                break;
            case 1:
                if (performedDash) {
                    Invoke("NextEvent", tooltipChangeDelay);
                    delayingChange = true;
                }
                break;
            case 2:
                if (performedLight) {
                    Invoke("NextEvent", tooltipChangeDelay);
                    delayingChange = true;
                }
                break;
            case 3:
                if (GlobalValues.Instance.fm.GetPlayerRoom(trackingPlayer1) != 0) {
                    Invoke("NextEvent", tooltipChangeDelay/2f);
                    delayingChange = true;
                }
                break;
            case 4:
                if (performedShoot) {
                    Invoke("NextEvent", tooltipChangeDelay);
                    delayingChange = true;
                }
                break;
            case 5:
                if (performedRightClick) {
                    Invoke("NextEvent", tooltipChangeDelay);
                    delayingChange = true;
                }
                break;
            case 6:
                if (tutorialBotEnemyTransform.childCount == 0) {
                    Invoke("NextEvent", tooltipChangeDelay);
                    delayingChange = true;
                }
                break;
            default:
                break;
        }
    }

}

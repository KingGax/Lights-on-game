using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleTutorialHelper : MonoBehaviour
{
    public List<string> tooltipMessages;
    public Tooltip tip;
    private int currentTooltipIndex = -1;
    bool finished = false;
    public float tooltipChangeDelay;
    private bool delayingChange = false;
    bool playerFound = false;
    public Vector3 offset;
    private GameObject player;
    private bool trackingPlayer1 = true;

    void NextEvent() {
        currentTooltipIndex++;
        switch (currentTooltipIndex) {
            default:
                break;
        }
        if (currentTooltipIndex < tooltipMessages.Count) {
            tip.Text = tooltipMessages[currentTooltipIndex];
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
                if (GlobalValues.Instance.fm.GetPlayerRoom(trackingPlayer1) > 1) {
                    Invoke("NextEvent", tooltipChangeDelay / 2f);
                    delayingChange = true;
                }
                break;
            default:
                break;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        tip = gameObject.GetComponentInChildren<Tooltip>();
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
}

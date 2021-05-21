using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class WinMenuHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerController.LocalPlayerInstance != null) {
            PhotonView pv = PlayerController.LocalPlayerInstance.GetPhotonView();
            if (pv != null && pv.IsMine) {
                PhotonNetwork.Destroy(PlayerController.LocalPlayerInstance);
            }
            if (GlobalValues.Instance.UIElements != null) {
                GlobalValues.Instance.UIElements.GetComponentInChildren<HealthBar>().gameObject.SetActive(false);
                GlobalValues.Instance.UIElements.GetComponentInChildren<TimerCountdownText>().gameObject.SetActive(false);
                GlobalValues.Instance.UIElements.GetComponentInChildren<HelpTooltip>().gameObject.SetActive(false);
                GlobalValues.Instance.UIElements.GetComponentInChildren<AmmoUI>().gameObject.SetActive(false);
                GlobalValues.Instance.UIElements.GetComponentInChildren<ObjectiveTextManager>().gameObject.SetActive(false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

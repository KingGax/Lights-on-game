using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LeaveScript : MonoBehaviour
{
    public void LeaveRoom() {
        PhotonNetwork.LeaveRoom();
    }
}

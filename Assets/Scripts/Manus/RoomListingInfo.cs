using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;

public class RoomListingInfo : MonoBehaviour
{


    [SerializeField]
    private Text text;



    private RoomInfo _roomInfo;

    public void SetRoomInfo(RoomInfo roomInfo)
    {
        _roomInfo = roomInfo;
        text.text = roomInfo.Name;
    }

    public void OnCLick_Button()
    {
        PhotonNetwork.JoinRoom(_roomInfo.Name);
    }
}

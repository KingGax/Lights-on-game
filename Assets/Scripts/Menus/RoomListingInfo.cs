using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;
using TMPro;

public class RoomListingInfo : MonoBehaviour
{


    [SerializeField]
    private TextMeshProUGUI text;

    private RoomInfo _roomInfo;

    public void SetRoomInfo(RoomInfo roomInfo)
    {
        _roomInfo = roomInfo;
        //string name;
        text.text = (string)roomInfo.CustomProperties["name"];
    }

    public void OnCLick_Button()
    {
        if(!string.IsNullOrEmpty(PhotonNetwork.NickName)){
            PhotonNetwork.JoinRoom(_roomInfo.Name);
        }
    }
}

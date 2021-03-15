using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;

public class PlayerListingInfo : MonoBehaviour
{


    [SerializeField]
    private Text text;

    

    private Player _player;

    public void SetPlayerInfo(Player player)
    {
        _player = player;
        text.text = player.NickName;

    }

    public void OnCLick_Button()
    {

    }
}

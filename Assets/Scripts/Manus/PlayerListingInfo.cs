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
    bool _spectator;

    

    private Player _player;

    public void SetPlayerInfo(Player player, bool spectator)
    {
        _player = player;
        text.text = player.NickName;
        _spectator = spectator;
    }

    public void OnCLick_Button()
    {

    }
}

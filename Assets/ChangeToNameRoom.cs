using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ChangeToNameRoom : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.LoadLevel("NameMenu");
    }

}

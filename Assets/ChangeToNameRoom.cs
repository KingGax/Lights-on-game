using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ChangeToNameRoom : MonoBehaviour
{
    // Start is called before the first frame update
    public Material lightCatcher;
    void Start()
    {
        PhotonNetwork.LoadLevel("NameMenu");
        Vector3 farAwayPosition = new Vector3(9999, 9999, 9999);
        lightCatcher.SetVector("Vector3_1", farAwayPosition);
        lightCatcher.SetVector("Vector3_8cf38a4ca0cb4f6589592a89d233cd7f", farAwayPosition);
    }

}

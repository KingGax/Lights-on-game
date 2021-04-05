using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ShaderPlayerTracker : MonoBehaviour {

    public Material shaderMaterial;

    private Lanturn p1Lantern;
    private Lanturn p2Lantern;
    private GameObject p1;
    private GameObject p2;
    private bool twoPlayers = false;
    private bool initialised = false;
    private int numberOfPlayers;

    void Start() {
        numberOfPlayers = PhotonNetwork.CurrentRoom.PlayerCount;
    }

    void Update() {
        if (!initialised) {
            if (GlobalValues.Instance.players.Count >= numberOfPlayers) {
                if (numberOfPlayers == 2) {
                    p2 = GlobalValues.Instance.players[1];
                    p2Lantern = p2.GetComponentInChildren<Lanturn>();
                    twoPlayers = true;
                } else {
                    shaderMaterial.SetColor("Color_1", new Color(0, 0, 0));
                    shaderMaterial.SetVector("Vector3_1", new Vector3(-100, -1000, -100));
                }
                initialised = true;
                p1 = GlobalValues.Instance.players[0];
                p1Lantern = p1.GetComponentInChildren<Lanturn>();
                shaderMaterial.SetFloat("Vector1_dc4d66f007f1473396bf01ec30d43ab3", p1Lantern.GetRange());
            }
        } else {
            if (twoPlayers) {
                shaderMaterial.SetColor("Color_1", p2Lantern.GetColour().DisplayColour());
                shaderMaterial.SetVector("Vector3_1", p2.transform.position);
            }
            shaderMaterial.SetVector("Vector3_8cf38a4ca0cb4f6589592a89d233cd7f", p1.transform.position);
            shaderMaterial.SetColor("Color_0e196a011788488595d0f269674a173d", p1Lantern.GetColour().DisplayColour());
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ShaderPlayerTracker : MonoBehaviour {
    LightObject p1Lantern;
    LightObject p2Lantern;
    GameObject p1;
    GameObject p2;
    bool twoPlayers = false;
    bool initialised = false;
    public Material shaderMaterial;
    int numberOfPlayers;
    // Start is called before the first frame update
    void Start() {
        numberOfPlayers = PhotonNetwork.CurrentRoom.PlayerCount;
    }

    // Update is called once per frame
    void Update() {
        if (!initialised) {
            if (GlobalValues.Instance.players.Count >= numberOfPlayers) {
                if (numberOfPlayers == 2) {
                    p2 = GlobalValues.Instance.players[1];
                    p2Lantern = p2.GetComponentInChildren<LightObject>();
                    twoPlayers = true;
                }
                else {
                    shaderMaterial.SetColor("Color_1", new Color(0, 0, 0));
                    shaderMaterial.SetVector("Vector3_1", new Vector3(-100, -1000, -100));
                }
                initialised = true;
                p1 = GlobalValues.Instance.players[0];
                p1Lantern = p1.GetComponentInChildren<LightObject>();
                shaderMaterial.SetFloat("Vector1_dc4d66f007f1473396bf01ec30d43ab3", p1Lantern.GetRange());
            }
        }
        else {
            if (twoPlayers) {
                shaderMaterial.SetColor("Color_1", p2Lantern.colour);
                shaderMaterial.SetVector("Vector3_1", p2.transform.position);
            }
            shaderMaterial.SetVector("Vector3_8cf38a4ca0cb4f6589592a89d233cd7f", p1.transform.position);
            shaderMaterial.SetColor("Color_0e196a011788488595d0f269674a173d", p1Lantern.colour);
        }
    }
}

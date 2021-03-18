using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderPlayerTracker : MonoBehaviour
{
    LightObject p1Lantern;
    GameObject p1;
    bool twoPlayers = false;
    bool initialised = false;
    public Material shaderMaterial;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!initialised) {
            if (GlobalValues.Instance.localPlayerInstance != null) {
                initialised = true;
                p1 = GlobalValues.Instance.players[0];
                p1Lantern = p1.GetComponentInChildren<LightObject>();
                shaderMaterial.SetFloat("Vector1_dc4d66f007f1473396bf01ec30d43ab3", p1Lantern.GetRange());
            }
        }
        else {
            if (twoPlayers) {

            }
            else {
                //Vector3__8cf38a4ca0cb4f6589592a89d233cd7f
                //Color_0e196a011788488595d0f269674a173d
                //Vector1_dc4d66f007f1473396bf01ec30d43ab3
                //shaderMaterial.SetVector("Vector3_8cf38a4ca0cb4f6589592a89d233cd7f", new Vector3(1, 2, 3));
                //shaderMaterial.SetColor("Color_0e196a011788488595d0f269674a173d", new Color(0,1,0));
                shaderMaterial.SetColor("Color_1", p1Lantern.colour);
                shaderMaterial.SetVector("Vector3_1", p1.transform.position);
            }
        }
    }
}

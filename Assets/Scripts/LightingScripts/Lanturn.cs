using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(Light))]
[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(PhotonView))]
public class Lanturn : MonoBehaviour {

    private LightableColour colour;
    private PhotonView pv;
    private Light light;
    private float lightRange;
    private float range;
    private SphereCollider sphere;
    private int lightLayer;

    public void Awake() {
        sphere = GetComponent<SphereCollider>();
        pv = GetComponent<PhotonView>();
        light = GetComponent<Light>();
        colour = LightableColour.Red;
    }

    public void Start() {
        lightRange = light.range;
        range = lightRange / 1.8f;
        sphere.radius = range;
        lightLayer = 1 << LayerMask.NameToLayer("LightingHitboxes");
    }

    public float GetRange() {
        return range;
    }

    public LightableColour GetColour() {
        return colour;
    }

    [PunRPC]
    private void UpdateColour(LightableColour col) {
        colour = col;
        light.color = colour.DisplayColour();
        Collider[] hitColliders = Physics.OverlapSphere(transform.position+sphere.center, sphere.radius,lightLayer);
        foreach (var hitCollider in hitColliders) {
            LightableObject ls = hitCollider.GetComponent<LightableObject>();
            if (ls != null) {
                ls.ColourChanged();
            }
        }
    }

    public void SetColour(LightableColour col) {
        if (pv == null || !pv.IsMine) {
            Debug.LogError("Tried to change colour of lanturn we do not own", gameObject);
            return;
        }
        pv.RPC("UpdateColour", RpcTarget.AllBuffered, col);
    }
}
using Photon.Pun;
using UnityEngine;

namespace LightsOn {
namespace LightingSystem {

[RequireComponent(typeof(Light))]
[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(PhotonView))]
public class Lanturn : MonoBehaviour {

    private LightColour colour;
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
        colour = LightColour.Red;
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

    public LightColour GetColour() {
        return colour;
    }
    public void BufferLightColour() {
        pv.RPC("UpdateColour", RpcTarget.AllBufferedViaServer, colour);
    }
            

    [PunRPC]
    private void UpdateColour(LightColour col) {
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

    public void SetColour(LightColour col) {
        if (pv == null || !pv.IsMine) {
            Debug.LogError("Tried to change colour of lanturn we do not own", gameObject);
            return;
        }
        pv.RPC("UpdateColour", RpcTarget.AllBuffered, col);
    }
}}}
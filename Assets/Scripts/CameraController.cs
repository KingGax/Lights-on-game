using UnityEngine;

public class CameraController : MonoBehaviour {
    public Transform player;
    public Vector3 offset;
    public Vector3 rotation;

    public void bindToPlayer(Transform viewOwner) {
        this.player = viewOwner;
    }

    void Start() {
        transform.rotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z);
    }

    void Update() {
        transform.position = player.transform.position + offset;
    }
}

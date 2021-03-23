using UnityEngine;

public class CameraWork : MonoBehaviour {

    [Tooltip("The distance in the local x-z plane to the target")]
    [SerializeField]
    private float distance = 17.0f;

    [Tooltip("The height we want the camera to be above the target")]
    [SerializeField]
    private float height = 9.0f;

    [Tooltip("Allow the camera to be offseted vertically from the target, for example giving more view of the sceneray and less ground.")]
    [SerializeField]
    private Vector3 centerOffset = Vector3.zero;

    [Tooltip("Set this as false if a component of a prefab being instanciated by Photon Network, and manually call OnStartFollowing() when and if needed.")]
    [SerializeField]
    private bool followOnStart = false;

    [Tooltip("The Smoothing for the camera to follow the target")]
    [SerializeField]
    private float smoothSpeed = 0.125f;
    private int playerIndex = 0;
    public Vector3 offset;

    // cached transform of the target
    Transform cameraTransform;
    Transform targetTransform;

    // maintain a flag internally to reconnect if target is lost or camera is switched
    bool isFollowing;

    // Cache for camera offset
    Vector3 cameraOffset = Vector3.zero;

    public void Start() {
        if (followOnStart) {
            OnStartFollowing();
        }
    }

    void LateUpdate() {
        // The transform target may not destroy on level load,
        // so we need to cover corner cases where the Main Camera is different everytime we load a new scene, and reconnect when that happens
        if (cameraTransform == null && isFollowing) {
            OnStartFollowing();
        }

        // only follow is explicitly declared
        if (isFollowing) {
            Follow ();
        }
    }

    public void SwitchPlayer() {
        playerIndex = (playerIndex + 1) % GlobalValues.Instance.players.Count;
        targetTransform = GlobalValues.Instance.players[playerIndex].transform; 
    }

    // Raises the start following event.
    // Use this when you don't know at the time of editing what to follow, typically instances managed by the photon network.
    public void OnStartFollowing() {
        cameraTransform = Camera.main.transform;
        targetTransform = transform;
        isFollowing = true;
        // we don't smooth anything, we go straight to the right camera shot
        Cut();
    }

    private void Follow() {
        cameraTransform.position = targetTransform.position + offset;
    }

    private void Cut() {
        cameraTransform.position = targetTransform.position + offset;
    }
}
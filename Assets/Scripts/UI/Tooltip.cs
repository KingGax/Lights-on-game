using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public enum TooltipOrientation {
    X, Z, Camera, CameraPlane
}

[System.Serializable]
[RequireComponent(typeof(PhotonView))]
public class Tooltip : MonoBehaviour {

    [SerializeField]
    private string text;
    [SerializeField]
    private TooltipOrientation orientation;
    private Transform target;
    private float t;
    protected PhotonView pv;

    public void Awake() {
        pv = gameObject.GetComponent<PhotonView>();
        target = transform;
        Text = text;
        Orientation = orientation;
    }

    public void Update() {
        t += Time.deltaTime;
        transform.position = target.position
            + new Vector3(0.0f, 0.01f * Mathf.Sin(t), 0.0f);
    }

    public void Dismiss() {
        gameObject.SetActive(false);
    }


    [PunRPC]
    protected void SetTextRPC(string text) {
        TextMeshProUGUI label = GetComponentInChildren<TextMeshProUGUI>();
        label.text = text;

        RectTransform canvas = GetComponentInChildren<RectTransform>();
        canvas.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (18 * text.Length) + 64);
    }

    public string Text {
        get {return text;}
        set {
            text = value;
            pv.RPC("SetTextRPC", RpcTarget.All, text);
        }
    }

    public TooltipOrientation Orientation{
        get {return orientation;}
        set {
            orientation = value;
            switch (value) {
                case TooltipOrientation.X:
                    transform.right = Vector3.forward;
                    break;
                case TooltipOrientation.Z:
                    transform.right = Vector3.left;
                    break;
                case TooltipOrientation.CameraPlane:
                    transform.rotation = Camera.main.transform.rotation;
                    break;
                case TooltipOrientation.Camera:
                default:
                    Vector3 angle = new Vector3(0.0f, 1.0f, 1.0f);
                    angle.Scale(Camera.main.transform.eulerAngles);
                    transform.eulerAngles = angle;
                    break;
            }
        }
    }
}
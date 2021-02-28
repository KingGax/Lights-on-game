using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum TooltipOrientation {
    X, Z, Camera
}

[System.Serializable]
public class Tooltip : MonoBehaviour {

    [SerializeField]
    private string text;
    [SerializeField]
    private TooltipOrientation orientation;
    private Transform target;
    private float t;

    public void Awake() {
        target = transform;
        Text = text;
        Orientation = orientation;
    }

    public void Update() {
        t += Time.deltaTime;
        transform.position = target.position
            + new Vector3(0.0f, 0.01f * Mathf.Sin(t), 0.0f);
    }

    public string Text {
        get {return text;}
        set {
            text = value;
            TextMeshProUGUI label = GetComponentInChildren<TextMeshProUGUI>();
            label.text = text;

            RectTransform canvas = GetComponentInChildren<RectTransform>();
            canvas.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (16 * text.Length) + 64);
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
                case TooltipOrientation.Camera:
                default:
                    transform.rotation = Camera.main.transform.rotation;
                    break;
            }
        }
    }
}
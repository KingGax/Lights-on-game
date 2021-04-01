using UnityEngine;

public class ColouredMaterial : MonoBehaviour {

    [SerializeField]
    protected Material red;
    [SerializeField]
    protected Material green;
    [SerializeField]
    protected Material blue;
    [SerializeField]
    protected Material magenta;
    [SerializeField]
    protected Material cyan;
    [SerializeField]
    protected Material yellow;
    [SerializeField]
    protected Material white;

    public void Awake() {
        GlobalValues gv = GlobalValues.Instance;
        if (green == null)
            green = gv.defaultGreen;

        if (red == null)
            red = gv.defaultRed;

        if (blue == null)
            blue = gv.defaultBlue;
    }

    public Material get(LightableColour col) {
        switch (col) {
            case LightableColour.Red:
                return red;
            case LightableColour.Green:
                return green;
            case LightableColour.Blue:
                return blue;
            case LightableColour.Cyan:
                return cyan;
            case LightableColour.Magenta:
                return magenta;
            case LightableColour.Yellow:
                return yellow;
            case LightableColour.White:
            default:
                return white;
        }
    }
}
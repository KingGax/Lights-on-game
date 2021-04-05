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

    public Material get(LightColour col) {
        switch (col) {
            case LightColour.Red:
                return red;
            case LightColour.Green:
                return green;
            case LightColour.Blue:
                return blue;
            case LightColour.Cyan:
                return cyan;
            case LightColour.Magenta:
                return magenta;
            case LightColour.Yellow:
                return yellow;
            case LightColour.White:
            default:
                return white;
        }
    }
}
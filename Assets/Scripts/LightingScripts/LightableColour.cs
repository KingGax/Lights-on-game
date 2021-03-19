using UnityEngine;

public enum LightableColour {
    Red = 0xff0000,
    Green = 0x00ff00,
    Blue = 0x0000ff,
    Cyan = 0x00ffff,
    Magenta = 0xff00ff,
    Yellow = 0xffff00,
    White = 0xffffff,
}
/*
 * 
 * Red = 0xff3d3d,
    Green = 0x3dff3d,
    Blue = 0x3d3dff,
    Cyan = 0x3dffff,
    Magenta = 0xff3dff,
    Yellow = 0xffff3d,
    White = 0xffffff,
 */

public static class LightableColourMethods {

    public static Color ToColor(this LightableColour colour) {
        switch (colour) {
            case LightableColour.Red:
                return new Color(1, 0, 0);
            case LightableColour.Green:
                return new Color(0, 1, 0);
            case LightableColour.Blue:
                return new Color(0, 0, 1);
            case LightableColour.Cyan:
                return new Color(0, 1, 1);
            case LightableColour.Magenta:
                return new Color(1, 0, 1);
            case LightableColour.Yellow:
                return new Color(1, 1, 0);
            case LightableColour.White:
            default:
                return new Color(1, 1, 1); ;
        }
    }

    public static LightableColour MergeWith(this LightableColour s1, LightableColour with) {
        Vector4 lightColour = (Vector4)with.ToColor() + (Vector4)s1.ToColor();
        lightColour = new Vector4(
            Mathf.Round(Mathf.Clamp(lightColour.x, 0.0f, 1.0f)),
            Mathf.Round(Mathf.Clamp(lightColour.y, 0.0f, 1.0f)),
            Mathf.Round(Mathf.Clamp(lightColour.z, 0.0f, 1.0f)),
            1.0f
        );
        int c = (int)(lightColour.x * 65280 + lightColour.y * 4080 + lightColour.z * 255);
        return (LightableColour)c;
    }
}
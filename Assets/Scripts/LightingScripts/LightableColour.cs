using UnityEngine;

public enum LightableColour {
    Black = 0x000000,
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
            case LightableColour.Black:
                return new Color(0,0,0);
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

    public static Color DisplayColour(this LightableColour colour) {
        switch (colour) {
            case LightableColour.Black:
                return new Color(0,0,0);
            case LightableColour.Red:
                return new Color(1, 0.1f, 0.1f);
            case LightableColour.Green:
                return new Color(0.1f, 1, 0.1f);
            case LightableColour.Blue:
                return new Color(0.1f, 0.1f, 1);
            case LightableColour.Cyan:
                return new Color(0.239f, 1, 1);
            case LightableColour.Magenta:
                return new Color(1, 0.239f, 1);
            case LightableColour.Yellow:
                return new Color(1, 1, 0.239f);
            case LightableColour.White:
            default:
                return new Color(1, 1, 1); ;
        }
    }

    public static Color DoorLightColour(this LightableColour colour) {
        switch (colour) {
            case LightableColour.Black:
                return new Color(0,0,0);
            case LightableColour.Red:
                return new Color(1, 0.1f, 0.1f);
            case LightableColour.Green:
                return new Color(0.1f, 1, 0.1f);
            case LightableColour.Blue:
                return new Color(0.1f, 0.1f, 1);
            case LightableColour.Cyan:
                return new Color(0.239f, 1, 1);
            case LightableColour.Magenta:
                return new Color(1, 0.239f, 1);
            case LightableColour.Yellow:
                return new Color(1, 1, 0.239f);
            case LightableColour.White:
            default:
                return new Color(1, 1, 1); ;
        }
    }

    public static LightableColour MergeWith(this LightableColour s1, LightableColour with) {
        Vector4 lightColour = with.ToColor() + s1.ToColor();
        lightColour = new Vector4(
            Mathf.Clamp01(lightColour.x),
            Mathf.Clamp01(lightColour.y),
            Mathf.Clamp01(lightColour.z),
            1.0f
        );
        lightColour = Vector4.Scale(lightColour, new Vector4(255, 255, 255, 1));
        int c = ((int)Mathf.Round(lightColour.x) << 16)
            + ((int)Mathf.Round(lightColour.y) << 8)
            + (int)Mathf.Round(lightColour.z);
        return (LightableColour)c;
    }
}
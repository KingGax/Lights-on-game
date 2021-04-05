using UnityEngine;

public enum LightColour {
    Black = 0x000000,
    Red = 0xff0000,
    Green = 0x00ff00,
    Blue = 0x0000ff,
    Cyan = 0x00ffff,
    Magenta = 0xff00ff,
    Yellow = 0xffff00,
    White = 0xffffff,
}


/* Red = 0xff3d3d,
 * Green = 0x3dff3d,
 * Blue = 0x3d3dff,
 * Cyan = 0x3dffff,
 * Magenta = 0xff3dff,
 * Yellow = 0xffff3d,
 * White = 0xffffff,
 */

public static class LightColourMethods {

    public static Color ToColor(this LightColour colour) {
        switch (colour) {
            case LightColour.Black:
                return new Color(0,0,0);
            case LightColour.Red:
                return new Color(1, 0, 0);
            case LightColour.Green:
                return new Color(0, 1, 0);
            case LightColour.Blue:
                return new Color(0, 0, 1);
            case LightColour.Cyan:
                return new Color(0, 1, 1);
            case LightColour.Magenta:
                return new Color(1, 0, 1);
            case LightColour.Yellow:
                return new Color(1, 1, 0);
            case LightColour.White:
            default:
                return new Color(1, 1, 1); ;
        }
    }

    public static Color DisplayColour(this LightColour colour) {
        switch (colour) {
            case LightColour.Black:
                return new Color(0,0,0);
            case LightColour.Red:
                return new Color(1, 0.1f, 0.1f);
            case LightColour.Green:
                return new Color(0.1f, 1, 0.1f);
            case LightColour.Blue:
                return new Color(0.1f, 0.1f, 1);
            case LightColour.Cyan:
                return new Color(0.239f, 1, 1);
            case LightColour.Magenta:
                return new Color(1, 0.239f, 1);
            case LightColour.Yellow:
                return new Color(1, 1, 0.239f);
            case LightColour.White:
            default:
                return new Color(1, 1, 1); ;
        }
    }

    public static Color DoorLightColour(this LightColour colour) {
        switch (colour) {
            case LightColour.Black:
                return new Color(0,0,0);
            case LightColour.Red:
                return new Color(1, 0.1f, 0.1f);
            case LightColour.Green:
                return new Color(0.1f, 1, 0.1f);
            case LightColour.Blue:
                return new Color(0.1f, 0.1f, 1);
            case LightColour.Cyan:
                return new Color(0.239f, 1, 1);
            case LightColour.Magenta:
                return new Color(1, 0.239f, 1);
            case LightColour.Yellow:
                return new Color(1, 1, 0.239f);
            case LightColour.White:
            default:
                return new Color(1, 1, 1); ;
        }
    }

    public static LightColour MergeWith(this LightColour s1, LightColour with) {
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
        return (LightColour)c;
    }
}
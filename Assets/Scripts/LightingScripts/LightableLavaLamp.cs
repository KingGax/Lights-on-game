using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightableLavaLamp : LightableObstacle {

    public MeshRenderer lavaMeshRenderer;
    public Material lavaGreen;
    public Material lavaBlue;
    public Material lavaRed;
    private Material lavaColour;
    public Animator anim;
    
    float animSpeed;

    public override void Start() {
        base.Start();
        animSpeed = Random.Range(0.95f, 1.05f);
        anim.speed = animSpeed;
        float startTime = Random.Range(0, 1f);
        anim.Play("lava_Time", 0, startTime);
    }

    public override void SetColour(LightColour col) {
        base.SetColour(col);
        switch (colour) {
            case LightColour.Red:
                lavaColour = lavaRed;
                break;
            case LightColour.Green:
                lavaColour = lavaGreen;
                break;
            case LightColour.Blue:
                lavaColour = lavaBlue;
                break;
            case LightColour.Cyan:
                break;
            case LightColour.Magenta:
                break;
            case LightColour.Yellow:
                break;
            case LightColour.White:
                break;
            default:
                break;
        }
        lavaMeshRenderer.material = lavaColour;
    }

    public override void Disappear() {
        base.Disappear();
        anim.speed = 0;
        lavaMeshRenderer.material = hiddenMaterials.get(colour);
    }

    public override void Appear() {
        base.Appear();
        anim.speed = animSpeed;
        lavaMeshRenderer.material = lavaColour;
    }
}

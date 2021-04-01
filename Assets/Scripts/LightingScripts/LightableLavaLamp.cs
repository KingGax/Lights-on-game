using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightableLavaLamp : LightableObstacle
{
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

    public override void SetColour() {
        base.SetColour();
        switch (colour) {
            case LightableColour.Red:
                lavaColour = lavaRed;
                break;
            case LightableColour.Green:
                lavaColour = lavaGreen;
                break;
            case LightableColour.Blue:
                lavaColour = lavaBlue;
                break;
            case LightableColour.Cyan:
                break;
            case LightableColour.Magenta:
                break;
            case LightableColour.Yellow:
                break;
            case LightableColour.White:
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

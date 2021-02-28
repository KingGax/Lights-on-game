using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingHealthBar : HealthBar
{
    Transform parentTransform;
    Canvas canvas;
    float hideTimer;
    public float hideTimerMax;
    // Start is called before the first frame update
    void Start() {
        parentTransform = gameObject.GetComponentInParent<Transform>();        
        canvas = gameObject.GetComponent<Canvas>();
        hideTimer = 0;
        StartCoroutine("CountdownTimers");
    }

    public override void UpdateHealth(float hp) {
        base.UpdateHealth(hp);
        if (hp != maxHealth) {
            canvas.enabled = true;
            hideTimer = hideTimerMax;
        }
        
    }

    void Update() {
        transform.rotation = Quaternion.Euler(0, -parentTransform.rotation.y-148.5f, 0);
        //transform.LookAt(Camera.main.transform.position);
        //transform.rotation = Quaternion.Euler(0f, transform.rotation.y, 0f);
    }

    private IEnumerator CountdownTimers() {
        while (true) {
            if (hideTimer > 0) {
                hideTimer -= Time.deltaTime;
                if (hideTimer <= 0) {
                    canvas.enabled = false;
                }
            }
            yield return null; 
        } 
    }
}

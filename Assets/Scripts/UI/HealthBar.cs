using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    protected Transform bar;
    //GameObject playerObj;
    protected float maxHealth;

    private void Awake() {
        bar = transform.Find("Bar");
        bar.localScale = new Vector3(1f, 1f);
        maxHealth = -100;
        // if (GlobalValues.Instance != null)
        // {
        //     playerObj = GlobalValues.Instance.localPlayerInstance;
        // }
    }

    public void SetPlayer(){
        //playerObj = GlobalValues.Instance.localPlayerInstance;
    }

    public void UpdateMaxHealth(float maxHP) {
        maxHealth = maxHP;
    }

    public virtual void UpdateHealth(float hp) {
        if (hp < 0) {
            hp = 0;
        }
        bar.localScale = new Vector3(hp/maxHealth, 1f);
    }
}

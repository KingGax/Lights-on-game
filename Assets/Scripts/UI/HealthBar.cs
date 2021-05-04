using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    protected Image bar;
    //GameObject playerObj;
    protected float maxHealth;

    private void Awake() {
        bar = transform.Find("Bar").Find("BarSprite").GetComponent<Image>();
        bar.fillAmount = 1;
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
        bar.fillAmount = hp/maxHealth;
    }
}

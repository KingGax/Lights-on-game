using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    private Transform bar;
    //GameObject playerObj;
    float maxHealth;
    // Start is called before the first frame update
    void Start()
    {
        
    }

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

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateMaxHealth(float maxHP){
        maxHealth = maxHP;
    }

    public void UpdateHealth(float hp){
        if (hp < 0){
            hp = 0;
        }
        bar.localScale = new Vector3(hp/maxHealth, 1f);
    }
}

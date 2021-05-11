using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using LightsOn.WeaponSystem;

public class AmmoUI : MonoBehaviour {
    int maxAmmo = 0;
    int ammo = 0;
    int displayedAmmo = 0;
    private PlayerWeapon currentWeapon;
    bool reloading = false;
    private TextMeshProUGUI text;
    bool initialised = false;
    GameObject player;
    PlayerController playerScript;

    private void Start() {
        text = GetComponentInChildren<TextMeshProUGUI>();
    }
    public void SetCurrentWeapon(PlayerWeapon wep) {
        currentWeapon = wep;
    }
    /*public void SetAmmo(int count) {
        ammo = count;
        UpdateDisplay();
    }
    public void SetMaxAmmo(int count) {
        maxAmmo = count;
        UpdateDisplay();
    }

    public void StartReloading() {
        reloading = true;
        UpdateDisplay();
    }

    public void StopReloading() {
        reloading = false;
        UpdateDisplay();
    }*/
    public void Update() {
        if (!initialised) {
            if (GlobalValues.Instance.localPlayerInstance != null) {
                player = GlobalValues.Instance.localPlayerInstance;
                playerScript = player.GetComponent<PlayerController>();
                initialised = true;
            }
        } else {
            currentWeapon = playerScript.equiptedWeapon;
            ammo = currentWeapon.GetAmmo();
            reloading = currentWeapon.IsReloading();
            maxAmmo = currentWeapon.maxAmmo;
            UpdateDisplay();
        }
    }

    public void UpdateDisplay() {
        if (reloading) {
            text.SetText("Reloading!");
        } else if (ammo != displayedAmmo) {
            text.SetText("Ammo: " + ammo.ToString() + "/" + maxAmmo.ToString());
            displayedAmmo = ammo;
        }        
    }
}
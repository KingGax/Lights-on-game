using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class ContactWeapon : Weapon {
    public float knockback;
    public float knockbackDuration;
    private PhotonView pv;

    bool active = false;
    ChargerEnemyController parentScript;
    List<Collider> alreadyHit = new List<Collider>();

    void Awake() {
        damage = 0f;
        knockback = 0f;
        knockbackDuration = 0f;
        parentScript = gameObject.GetComponentInParent<ChargerEnemyController>();
        pv = gameObject.GetPhotonView();
    }

    [PunRPC]
    protected void UseWeaponRPC()
    {
        active = true;
        alreadyHit.Clear();
    }
    protected override void UseWeapon()
    {
        pv.RPC("UseWeaponRPC",RpcTarget.All);
    }

    public void Deactivate() {
        active = false;
        alreadyHit.Clear();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject == GlobalValues.Instance.localPlayerInstance)
        {
            if (active && !frozen)
            {
                if (!alreadyHit.Contains(other))
                {
                    alreadyHit.Add(other);
                    Health ds = other.gameObject.GetComponent<Health>();
                    if (ds != null)
                    {
                        ds.Damage(damage);
                        IKnockbackable ks = other.gameObject.GetComponent<IKnockbackable>();
                        if (ks != null)
                        {
                            Vector3 dir = gameObject.transform.forward; // this might need to be changed
                            ks.TakeKnockback(dir, knockback, knockbackDuration);
                        }
                        parentScript.ChangeToChargeEnd();
                    }
                }
            }
        }
    }
}

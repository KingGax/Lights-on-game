using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightsOn.HealthSystem;
using Photon.Pun;
public class MissileCollisionDetection : MonoBehaviour, IDamagingProjectile
{
    MissileController missile;
    PhotonView pv;
    LayerMask walls;
    // Start is called before the first frame update
    void Start()
    {
        missile = transform.parent.GetComponent<MissileController>();
        pv = transform.parent.GetComponent<PhotonView>();
        walls = GlobalValues.Instance.environment;
    }

    public float GetDamage() {
        return missile.damage;
    }

    public float GetHitstun() {
        return missile.hitstun;
    }

    public void HitPlayer() {
        missile.Detonate();
    }

    private void OnTriggerEnter(Collider other) {
        if (((1 << other.gameObject.layer) & walls) != 0) {
            if (pv == null || !pv.IsMine) return;
            missile.Detonate();
        }
    }


}

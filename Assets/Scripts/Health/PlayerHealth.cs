using UnityEngine;
using Photon.Pun;

public sealed class PlayerHealth : Health {
    int bulletLayer;
    private void Awake()
    {

    }

    public override void Die() {
        Debug.Log("my healh has depleted to zero but there is not a game over scene aaaaaa");
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == bulletLayer) {
            BulletController bc = other.gameObject.GetComponent<BulletController>();
            if (bc != null) {
                Damage(bc.damage);
                PhotonView bulletPv = other.gameObject.GetPhotonView();
                bulletPv.TransferOwnership(PhotonNetwork.LocalPlayer);
                bc.DestroyBullet();
            } else {
                Debug.LogError("Something on bullet layer does not have bullet controller");
            }
        }
    }
}
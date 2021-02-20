using UnityEngine;
using Photon.Pun;

public class EnemyGun : Gun {

    private string bulletStr;

    public void SetColour(LightableColour col) {
        switch (col) {
            case LightableColour.Red:
                bulletStr = "RedEnemyBullet";
                break;
            case LightableColour.Green:
                bulletStr = "GreenEnemyBullet";
                break;
            case LightableColour.Blue:
                bulletStr = "BlueEnemyBullet";
                break;
            default:
                break;
        }
    }

    protected override void UseWeapon() {
        Vector3 direction = GlobalValues.Instance.players[0].transform.position - firePoint.position;
       
        GameObject newBullet = PhotonNetwork.Instantiate(bulletStr, firePoint.position, transform.rotation);
        BulletController bc = newBullet.GetComponent<BulletController>();
        bc.Fire(damage, bulletSpeed, direction);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightsOn.LightingSystem;
using Photon.Pun;
using Photon.Realtime;

public class SingleplayerDoorUnlocker : MonoBehaviour
{
    public List<LightableExitDoor> doors;
    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.OfflineMode) {
            foreach (LightableExitDoor door in doors) {
                if (door.GetColour() == LightColour.White) {
                    door.SetColour(LightColour.Yellow);
                }
                if (door.unlockedColour == LightColour.Cyan) {
                    door.unlockedColour = LightColour.Blue;
                }
                if (door.unlockedColour == LightColour.Magenta) {
                    door.unlockedColour = LightColour.Green;
                }
                if (door.unlockedColour == LightColour.Yellow) {
                    door.unlockedColour = LightColour.Red;
                }
            }
        }
    }
}

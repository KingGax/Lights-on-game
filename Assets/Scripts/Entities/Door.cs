using UnityEngine;
using LightsOn.LightingSystem;

public class Door : MonoBehaviour {

    [SerializeField]
    protected bool Locked;
    [SerializeField]
    protected LightColour colour;

    protected LightableObject lo;

    // Should these be RPC calls?? 
    // Advantage: guarantees that door locking will be networked
    // Disadvantage: one rpc call for each door unlock for room objective
    public void LockDoor() {
        Locked = true;
        lo.SetColour(LightColour.White);

    }

    public void UnLockDoor() {
        Locked = false;
        lo.SetColour(colour);
    }

    // This will be to update the colour of the door based on the ball colliding
    // This will be RPC'd
    public void SetColour() {

    }
}
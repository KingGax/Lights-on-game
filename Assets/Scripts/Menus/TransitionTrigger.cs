using UnityEngine;


public class TransitionTrigger : MonoBehaviour {
    public Animator transition;
    public void mouseClick() {
        transition.SetTrigger("Start");
    }
}
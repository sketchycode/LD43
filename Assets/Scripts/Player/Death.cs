using UnityEngine;

public class Death : MonoBehaviour {
    public void Kill() {
        GameManager.PlayerDied();
    }    
}
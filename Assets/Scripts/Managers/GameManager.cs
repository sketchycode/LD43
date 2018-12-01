using System;
using UnityEngine;

public class GameManager : MonoBehaviour {
	public static Action PlayerDied = delegate { };

    private void Start() {
        PlayerDied += OnPlayerDied;
    }

    private void OnPlayerDied() {
        // reset scene? wait for ok?
        Debug.Log("got signal for player death");
    }
}
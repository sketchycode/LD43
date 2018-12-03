using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinManager : MonoBehaviour {
	private void Start() {
		PlayerPrefs.DeleteKey("lastLevel");
	}

	private void Update() {
		if(Input.GetKeyDown(KeyCode.R)) {
			SceneManager.LoadScene(0);
		}
	}
}

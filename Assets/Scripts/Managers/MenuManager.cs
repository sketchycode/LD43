﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour {
	void Update() {
		if(Input.GetKeyDown(KeyCode.Q)) {
			PlayerPrefs.DeleteAll();
		}
	}
	
	public void PlayGame() {
		var level = PlayerPrefs.GetInt("lastLevel", 1);
		SceneManager.LoadScene(level);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {
	public static AudioManager Instance;

	private void Awake() {
		if(Instance != null) {
			GameObject.Destroy(gameObject);
		}
		else {
			Instance = this;
		}
	}

	public AudioClip test;
}

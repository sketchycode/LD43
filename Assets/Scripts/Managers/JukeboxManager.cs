using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JukeboxManager : MonoBehaviour {
	public static JukeboxManager Instance;
	private void Start() {
		if(Instance == null) {
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else {
			GameObject.Destroy(gameObject);
		}
	}
}

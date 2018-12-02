using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
	public FadeControl fade;
	// Use this for initialization
	void Start () {
		StartCoroutine(fade.FadeIn());
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathTouch : MonoBehaviour {
	public AudioSource touchSound;

	void OnTriggerEnter2D(Collider2D collision) {
		if(touchSound) {
			touchSound.Play();
		}
		collision.gameObject.SendMessage("Die", SendMessageOptions.DontRequireReceiver);
    }
}

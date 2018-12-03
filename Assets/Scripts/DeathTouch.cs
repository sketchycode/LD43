using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathTouch : MonoBehaviour {
	void OnTriggerEnter2D(Collider2D collision) {
		collision.gameObject.SendMessage("Die", SendMessageOptions.DontRequireReceiver);
    }
}

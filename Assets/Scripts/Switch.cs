﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour {
	public Transform targetTransform;
	public Rigidbody2D rigid2D;
	public float depressLimit;
	public Interactable[] actionObjects;
	public bool stayDepressed;
	public bool wasDepressed;

	private Vector3 switchStartPosition;
	public bool swapTriggers = false;

	public AudioSource switchSound;
	public AudioClip switchDown;
	public AudioClip switchUp;
	
	// Update is called once per frame
	void Update () {
		if(!wasDepressed) {
			if(isDepressed()) {
				performDepressedAction();
			}
		} else if(!stayDepressed) {
			if(!isDepressed()) {
				performUnpressedAction();
			}
		}
	}

	void PlayDepressedSound() {
		switchSound.PlayOneShot(switchDown);
	}

	void PlayUnpressedSound() {
		switchSound.PlayOneShot(switchUp);
	}

	void performAction(bool doEnable) {
		if(swapTriggers) {
			doEnable = !doEnable;
		}

		if(doEnable) {
			foreach(Interactable thing in actionObjects) {
				thing.Enable();
			}
		} else {
			foreach(Interactable thing in actionObjects) {
				thing.Disable();
			}
		}	
	}

	void performDepressedAction() {
		wasDepressed = true;
		performAction(true);
		PlayDepressedSound();
		if(stayDepressed) {
			rigid2D.isKinematic = true;
			rigid2D.velocity = Vector2.zero;
		}
	}

	void performUnpressedAction() {
		wasDepressed = false;
		PlayUnpressedSound();
		performAction(false);
	}

	bool isDepressed() {
		return getDepressionAmount() < depressLimit;
	}

	float getDepressionAmount() {
		return targetTransform.localPosition.y;
	}

	void OnDrawGizmosSelected() {
		Gizmos.color = Color.red;
		Gizmos.DrawLine(
			targetTransform.position + new Vector3(-1, depressLimit - 0.25f, 0), 
			targetTransform.position + new Vector3(1, depressLimit - 0.25f, 0)
		);
	}
}

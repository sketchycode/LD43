using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour {
	public Transform targetTransform;
	public Rigidbody2D rigid2D;
	public float depressLimit;
	public Interactable actionObject;
	public bool stayDepressed;
	public bool wasDepressed;

	public bool swapTriggers = false;
	
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

	void performAction(bool doEnable) {
		if(swapTriggers) {
			doEnable = !doEnable;
		}

		if(doEnable) {
			Debug.Log("Press");
			actionObject.Enable();
		} else {
			Debug.Log("UN-Press");
			actionObject.Disable();
		}
	}

	void performDepressedAction() {
		wasDepressed = true;
		performAction(true);
		if(stayDepressed) {
			rigid2D.isKinematic = true;
		}
	}

	void performUnpressedAction() {
		wasDepressed = false;
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour {
	public Rigidbody2D rigid2D;
	public float depressLimit;
	public GameObject actionObject;
	public bool stayDepressed;
	public bool wasDepressed;
	
	// Update is called once per frame
	void Update () {
		if(!wasDepressed) {
			if(isDepressed()) {
				performDepressedAction();
			}
		} else if(!stayDepressed) {
			if(!isDepressed()) {
				wasDepressed = false;
			}
		}
	}

	void performDepressedAction() {
		wasDepressed = true;
		Debug.Log("send action to ActionObject");
		if(stayDepressed) {
			rigid2D.isKinematic = true;
		}
	}

	bool isDepressed() {
		return getDepressionAmount() < depressLimit;
	}

	float getDepressionAmount() {
		return transform.localPosition.y;
	}

	void OnDrawGizmosSelected() {
		Gizmos.color = Color.red;
		Gizmos.DrawLine(
			transform.position + new Vector3(-1, depressLimit - 0.25f, 0), 
			transform.position + new Vector3(1, depressLimit - 0.25f, 0)
		);
	}
}

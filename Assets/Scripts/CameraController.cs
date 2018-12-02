using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
	public Transform target;
	public float cameraSmoothing = 1.0f;
	public FadeControl fade;
	public Vector2 cameraBoundsOffset;

	private Vector3 smoothVelocity;
	// Use this for initialization
	void Start () {
		StartCoroutine(fade.FadeIn());
		GameManager.Instance.SetCamera(this);
	}

	void Update() {
		TrackTarget();
	}

	public void SetTarget(Transform newTarget) {
		target = newTarget;
	}

	void TrackTarget() {
		if(target) {
			if(isTargetOutOfCameraBounds()) {
				Vector3 desiredPosition = target.position;
				desiredPosition.z = transform.position.z;
				transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref smoothVelocity, cameraSmoothing);
			} else {
				smoothVelocity = Vector3.zero;
			}
		}
	}

	bool isTargetOutOfCameraBounds() {
		return target.position.x > (transform.position.x + cameraBoundsOffset.x)
			|| target.position.x < (transform.position.x - cameraBoundsOffset.x)
			|| target.position.y > (transform.position.y + cameraBoundsOffset.y)
			|| target.position.y < (transform.position.y - cameraBoundsOffset.y);
	}
}

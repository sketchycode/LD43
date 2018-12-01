using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Controller : MonoBehaviour {
	[SerializeField] private float maxSpeed;
	[SerializeField] private float minSpeed;
	[SerializeField] private float maxMass;
	[SerializeField] private float minMass;
	[SerializeField] private float massLossWhileMoving;
	[SerializeField] private float jumpSpeedMax;
	[SerializeField] private float jumpSpeedMin;
	[SerializeField] private float slingShotScalingFactor;
	
	private Rigidbody2D rb2D;
	[SerializeField] private float mass;

	private bool slingStarted = false;
	private Vector2 slingStartPoint;
	private Vector2 slingJumpVelocity;

	public Action<JumpEvent> JumpTriggered = delegate { };
	public Action<MassChangeEvent> MassLost = delegate { };

	void Start() {
		rb2D = GetComponent<Rigidbody2D>();
		mass = maxMass;
	}

	void Update () {
		var inputVec = Vector2.zero;
		
		if(Input.GetKey(KeyCode.A)) {
			inputVec += Vector2.left;
		}
		if(Input.GetKey(KeyCode.D)) {
			inputVec += Vector2.right;
		}

		if(inputVec != Vector2.zero) {
			var speed = Mathf.Lerp(maxSpeed, minSpeed, Mathf.InverseLerp(minMass, maxMass, mass));
			var curVelocity = rb2D.velocity;
			rb2D.velocity = rb2D.velocity += inputVec * speed;
		}

		float velY = rb2D.velocity.y;
		rb2D.velocity = Vector2.zero;
		rb2D.velocity += (Vector2.up * velY) + slingJumpVelocity;
		slingJumpVelocity = Vector2.zero;
		mass -= rb2D.velocity.magnitude * massLossWhileMoving * Time.deltaTime;
	}

	private void OnMouseDown() {
		slingStarted = true;
		slingStartPoint = Input.mousePosition;
	}

	private void OnMouseUp() {
		if(slingStarted) {
			slingJumpVelocity = slingStartPoint - (Vector2)Input.mousePosition;
			slingJumpVelocity /= slingShotScalingFactor;
			var slingVelMgnt = slingJumpVelocity.magnitude;
			if(slingVelMgnt < jumpSpeedMin) {
				slingJumpVelocity = Vector2.zero;
			}
			else {
				if(slingVelMgnt > jumpSpeedMax) {
					slingJumpVelocity = slingJumpVelocity.normalized * jumpSpeedMax;
				}

				JumpTriggered(new JumpEvent() { JumpVelocity = slingJumpVelocity } );
				Debug.Log($"slingshot triggered: {slingJumpVelocity} [{slingJumpVelocity.magnitude}]");
			}
		}
	}

	private void OnSlingShotJump(Vector2 jumpVel) {
		slingJumpVelocity = jumpVel;
	}

	private void HandleSlingShotEvent(Vector2 slingDragVec) {
		var jumpVel = slingDragVec / slingShotScalingFactor;
		var jumpSpeed = jumpVel.magnitude;
		if(jumpSpeed < jumpSpeedMin) {
			jumpVel = Vector2.zero;
			return;
		}
		else {
			if(jumpSpeed > jumpSpeedMax) {
				jumpVel = jumpVel.normalized * jumpSpeedMax;
			}

			Debug.Log($"slingshot triggered: {jumpVel} [{jumpVel.magnitude}]");
			JumpTriggered(new JumpEvent() { JumpVelocity = jumpVel } );
		}
	}
}

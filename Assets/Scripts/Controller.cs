using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class Controller : MonoBehaviour {
	[Header("Speed")]
	[SerializeField] private float maxSpeed;
	[SerializeField] private float minSpeed;
	[Header("Mass")]
	[SerializeField] private float maxMass;
	[SerializeField] private float minMass;
	[SerializeField] private float massLossWhileMoving;
	[SerializeField] private float massLossWhileJumping;
	[Header("Jumping")]
	[SerializeField] private float jumpSpeedMax;
	[SerializeField] private float jumpSpeedMin;
	[SerializeField] private float slingShotScalingFactor;
	
	[Header("Readonly-no touchy")]
	[SerializeField] private float mass;

	private Rigidbody2D rb2D;
	new private BoxCollider2D collider2D;
	private bool slingStarted = false;
	private Vector2 slingStartPoint;
	private Vector2 slingJumpVelocity;
	private CollisionInfo collisionInfo = new CollisionInfo();
	private RaycastHit2D[] colliderHits = new RaycastHit2D[10];

	public Action<JumpEvent> JumpTriggered = delegate { };
	public Action<MassChangeEvent> MassLost = delegate { };

	void Start() {
		rb2D = GetComponent<Rigidbody2D>();
		collider2D = GetComponent<BoxCollider2D>();
		mass = maxMass;

		JumpTriggered += OnSlingShotJump;
		MassLost += OnMassLost;
	}

	void Update () {
		UpdateCollisions();
		var inputVec = Vector2.zero;
		
		if(collisionInfo.IsGrounded) {
			if(Input.GetKey(KeyCode.A)) {
				inputVec += Vector2.left;
			}
			if(Input.GetKey(KeyCode.D)) {
				inputVec += Vector2.right;
			}

			if(inputVec != Vector2.zero) {
				var speed = Mathf.Lerp(maxSpeed, minSpeed, Mathf.InverseLerp(minMass, maxMass, mass));
				rb2D.velocity = (Vector2.up * rb2D.velocity.y) + (inputVec * speed);

				var massDelta = massLossWhileMoving * Time.deltaTime;
				HandleMassLost(massDelta, MassChangeSourceType.Moving);
			}
		}

		rb2D.velocity += slingJumpVelocity;
		slingJumpVelocity = Vector2.zero;
	}

	private void OnMouseDown() {
		slingStarted = true;
		slingStartPoint = Input.mousePosition;
	}

	private void OnMouseUp() {
		if(slingStarted) {
			slingJumpVelocity = slingStartPoint - (Vector2)Input.mousePosition;
			HandleSlingShotEvent(slingJumpVelocity);
		}
	}

	private void OnSlingShotJump(JumpEvent e) {
		Debug.Log($"slingshot triggered: {e.JumpVelocity} [{e.JumpVelocity.magnitude}]");
		slingJumpVelocity = e.JumpVelocity;
		
		var massLoss = e.JumpVelocity.magnitude * massLossWhileJumping;
		HandleMassLost(massLoss, MassChangeSourceType.Jumping);
	}

	private void OnMassLost(MassChangeEvent e) {
		rb2D.mass = e.NewMass;
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

			JumpTriggered(new JumpEvent() { JumpVelocity = jumpVel } );
		}
	}

	private void HandleMassLost(float massLoss, MassChangeSourceType massChangeSource) {
		mass -= massLoss;
		var massChangeEvent = new MassChangeEvent() {
			MassDelta = -massLoss,
			NewMass = mass,
			MassChangeSource = massChangeSource
		};
		MassLost(massChangeEvent);
	}

	private void UpdateCollisions() {
		collisionInfo.Reset();

		Vector2 bottomCenter = new Vector2(collider2D.bounds.center.x, collider2D.bounds.min.y);
		Vector2 size = new Vector2(collider2D.size.x, 0.1f);
		var hitCounts = Physics2D.BoxCastNonAlloc(bottomCenter, size, 0, Vector2.up, colliderHits);
		Debug.DrawLine(bottomCenter, bottomCenter + Vector2.down * 0.1f, Color.red, 0.5f);
		if(hitCounts > 0) {
			for(int i=0; i<hitCounts; i++) {
				if(colliderHits[i].transform != transform) {
					collisionInfo.IsGrounded = true;
					return;
				}
			}
		}
	}
}

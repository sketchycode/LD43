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
	[SerializeField] private float keyboardJumpSpeed = 4f;
	[SerializeField] private float airMovementControlFactor = 0.2f;
	[Header("Collision")]
	[SerializeField] private float groundCheckBoxHeight = 0.1f;
	
	[Header("Readonly-no touchy")]
	[SerializeField] private float mass;
	[SerializeField] private CollisionInfo collisionInfo = new CollisionInfo();

	private Rigidbody2D rb2D;
	new private BoxCollider2D collider2D;
	private bool slingStarted = false;
	private Vector2 slingStartPoint;
	private Vector2 queuedJumpVelocity;
	private RaycastHit2D[] colliderHits = new RaycastHit2D[10];
	private bool isMoving = false;
	private bool isMovingRight = true;

	public Action<JumpEvent> JumpTriggered = delegate { };
	public Action<MassChangeEvent> MassLost = delegate { };

	public bool JustLanded => collisionInfo.JustGrounded;
	public bool JustJumped => collisionInfo.WasGrounded && !collisionInfo.IsGrounded;
	public bool IsMoving => isMoving && IsGrounded;
	public bool IsMovingRight => isMovingRight;
	public bool IsGrounded => collisionInfo.IsGrounded;
	public bool CanJump => IsGrounded;

	void Start() {
		rb2D = GetComponent<Rigidbody2D>();
		collider2D = GetComponent<BoxCollider2D>();
		mass = maxMass;

		JumpTriggered += OnJump;
		MassLost += OnMassLost;
	}

	void Update () {
		UpdateCollisions();
		var inputVel = 0f;
		isMoving = false;

		if(CanJump && Input.GetKey(KeyCode.Space)) {
			HandleJump(Vector2.up * keyboardJumpSpeed);
		}
		
		if(Input.GetKey(KeyCode.A)) {
			inputVel -= 1f;
		}
		if(Input.GetKey(KeyCode.D)) {
			inputVel += 1f;
		}

		if(inputVel != 0) {
			var speed = Mathf.Lerp(maxSpeed, minSpeed, Mathf.InverseLerp(minMass, maxMass, mass));
			inputVel *= (IsGrounded ? 1f : airMovementControlFactor) * speed;
			if(Mathf.Sign(inputVel) != Mathf.Sign(rb2D.velocity.x)) {
				rb2D.velocity = new Vector2(rb2D.velocity.x + inputVel, rb2D.velocity.y);
			}
			else if(Mathf.Abs(speed) > Mathf.Abs(rb2D.velocity.x)) {
				rb2D.velocity = new Vector2(inputVel, rb2D.velocity.y);
			}

			var massDelta = massLossWhileMoving * Time.deltaTime;
			HandleMassLost(massDelta, MassChangeSourceType.Moving);
		}

		rb2D.velocity += queuedJumpVelocity;
		queuedJumpVelocity = Vector2.zero;

		isMoving = Mathf.Abs(rb2D.velocity.x) > 0.2f;
		if(isMoving) {
			isMovingRight = rb2D.velocity.x > 0;
		}
	}

	private void OnMouseDown() {
		slingStarted = true;
		slingStartPoint = Input.mousePosition;
	}

	private void OnMouseUp() {
		if(slingStarted) {
			HandleSlingShotEvent(slingStartPoint - (Vector2)Input.mousePosition);
		}
	}

	private void OnJump(JumpEvent e) {
		Debug.Log($"jump triggered: {e.JumpVelocity} [{e.JumpVelocity.magnitude}]");
		queuedJumpVelocity = e.JumpVelocity;
		
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
			HandleJump(jumpVel);
		}
	}

	private void HandleJump(Vector2 jumpVel) {
		var jumpMassScale = Mathf.Clamp(Mathf.InverseLerp(maxMass, minMass, mass), 0.25f, 1f);
		JumpTriggered(new JumpEvent() { JumpVelocity = jumpVel * jumpMassScale } );
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
		Vector2 size = new Vector2(collider2D.size.x, groundCheckBoxHeight);
		var hitCounts = Physics2D.BoxCastNonAlloc(bottomCenter, size, 0, Vector2.zero, colliderHits);
		if(hitCounts > 0) {
			for(int i=0; i<hitCounts; i++) {
				if(colliderHits[i].transform != transform) {
					collisionInfo.IsGrounded = true;
					return;
				}
			}
		}
	}

	private void OnDrawGizmosSelected() {
		if(collider2D != null) {
			Gizmos.color = Color.red;
			Vector2 bottomCenter = new Vector2(collider2D.bounds.center.x, collider2D.bounds.min.y);
			Vector2 size = new Vector2(collider2D.size.x, groundCheckBoxHeight);
			Gizmos.DrawWireCube(bottomCenter, new Vector2(collider2D.size.x, groundCheckBoxHeight));
		}
	}
}
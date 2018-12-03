using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class Controller : MonoBehaviour {
	[Header("Speed")]
	[SerializeField] private float maxSpeed;
	[SerializeField] private float minSpeed;
	[SerializeField] private float inSlimeSpeedFactor = 1.3f;
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
	[Header("Misc")]
	[SerializeField] private GameObject slimeChunkPrefab;
	[SerializeField] private float visualMinMass = 0.4f;
	
	[Header("Readonly-no touchy")]
	[SerializeField] private float trueMass;
	[SerializeField] private float visualMass; // interpolated value
	[SerializeField] private CollisionInfo collisionInfo = new CollisionInfo();

	private Rigidbody2D rb2D;
	new private BoxCollider2D collider2D;
	private bool slingStarted = false;
	private Vector2 slingStartPoint;
	private Vector2 queuedJumpVelocity;
	private RaycastHit2D[] colliderHits = new RaycastHit2D[10];
	private bool isMoving = false;
	private bool isMovingRight = true;
	private bool isDead = false;
	private Tilemap platformsTilemap;
	private Vector3Int lastTileTouched = new Vector3Int(-9999, -9999, 0);

	public Action<JumpEvent> JumpTriggered = delegate { };
	public Action<MassChangeEvent> MassLost = delegate { };

	public bool JustLanded => collisionInfo.JustGrounded;
	public bool JustJumped => collisionInfo.WasGrounded && !collisionInfo.IsGrounded;
	public bool IsMoving => isMoving && IsGrounded;
	public bool IsMovingRight => isMovingRight;
	public bool IsGrounded => collisionInfo.IsGrounded;
	public bool IsDead => isDead;
	public bool CanJump => IsGrounded;
	public bool IsInSlime => OccupiedSlimes.Count > 0;

	public HashSet<SlimePatch> OccupiedSlimes = new HashSet<SlimePatch>();

	[Header("Sounds")]
	public AudioSource playerSound;
	public AudioClip moveSound;
	public AudioClip jumpSound;
	public AudioClip landSound;
	
	public AudioClip dieSound;

	void Start() {
		rb2D = GetComponent<Rigidbody2D>();
		collider2D = GetComponent<BoxCollider2D>();
		platformsTilemap = FindObjectOfType<Tilemap>();
		trueMass = maxMass;
		visualMass = 1f;

		JumpTriggered += OnJump;
		MassLost += OnMassLost;
	}

	void Update () {
		if(!isDead) {
			UpdateVisualMass();
			CheckForOtherThings();
		}
	}

	void FixedUpdate() {
		UpdateCollisions();
		UpdateMovement();
		if(JustLanded) {
			PlayOnce(landSound);
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
		PlayOnce(jumpSound);
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
		var jumpMassScale = Mathf.Clamp(Mathf.InverseLerp(maxMass, minMass, trueMass), 0.66f, 1f);
		JumpTriggered(new JumpEvent() { JumpVelocity = jumpVel * jumpMassScale } );
	}

	private void HandleMassLost(float massLoss, MassChangeSourceType massChangeSource) {
		trueMass -= massLoss;
		var massChangeEvent = new MassChangeEvent() {
			MassDelta = -massLoss,
			NewMass = trueMass,
			MassChangeSource = massChangeSource
		};
		MassLost(massChangeEvent);
		PlaySound(moveSound);
		if(trueMass <= minMass) {
			Die();
		}
	}

	private void UpdateCollisions() {
		collisionInfo.Reset();

		Vector2 bottomCenter = new Vector2(collider2D.bounds.center.x, collider2D.bounds.min.y);
		Vector2 size = new Vector2(collider2D.size.x * transform.localScale.x, groundCheckBoxHeight);
		var hitCounts = Physics2D.BoxCastNonAlloc(bottomCenter, size, 0, Vector2.zero, colliderHits);
		if(hitCounts > 0) {
			for(int i=0; i<hitCounts; i++) {
				if(colliderHits[i].transform != transform) {
					collisionInfo.IsGrounded = true;
					if(colliderHits[i].transform.name == "Tilemap") {
						PlaceSlimePatch(colliderHits[i]);
					}
				}
			}
		}
	}

	private void UpdateMovement() {
		var inputVel = 0f;
		isMoving = false;

		if(CanJump && Input.GetKeyDown(KeyCode.Space)) {
			HandleJump(Vector2.up * keyboardJumpSpeed);
		}
		
		if(Input.GetKey(KeyCode.A)) {
			inputVel -= 1f;
		}
		if(Input.GetKey(KeyCode.D)) {
			inputVel += 1f;
		}

		if(inputVel != 0) {
			var speed = Mathf.Lerp(maxSpeed, minSpeed, Mathf.InverseLerp(minMass, maxMass, trueMass));
			speed *= IsInSlime ? inSlimeSpeedFactor : 1f;
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
		else if(IsGrounded && !IsInSlime) {
			rb2D.velocity = new Vector2(0, rb2D.velocity.y);
		}

		rb2D.velocity += queuedJumpVelocity;
		queuedJumpVelocity = Vector2.zero;

		isMoving = Mathf.Abs(rb2D.velocity.x) > 0.2f;
		if(isMoving) {
			isMovingRight = rb2D.velocity.x > 0;
		}
	}

	private void UpdateVisualMass() {
		var interpMass = Mathf.InverseLerp(minMass, maxMass, trueMass);
		var interpVisualMass = Mathf.Lerp(visualMinMass, 1, interpMass);
		visualMass = Mathf.Lerp(visualMass, interpVisualMass, 0.05f);
		transform.localScale = Vector3.one * visualMass;
	}

	private void CheckForOtherThings() {
		if(Input.GetKeyDown(KeyCode.S)) {
			BirthSlimeBaby();
		}
		if(Input.GetKey(KeyCode.Escape)) {
			GameManager.OpenPauseMenu();
		}
	}

	private void PlaceSlimePatch(RaycastHit2D hit2D) {
		var hitCell = platformsTilemap.WorldToCell(new Vector2(transform.position.x, hit2D.point.y));
		if(hitCell != lastTileTouched || collisionInfo.JustGrounded) {
			if(collisionInfo.JustGrounded) { lastTileTouched = hitCell; }
			if(platformsTilemap.GetTile(lastTileTouched) != null) {
				GameManager.Instance.PlaceSlimeAtTilePosition(new Vector2Int(lastTileTouched.x, lastTileTouched.y));
			}
			lastTileTouched = hitCell;
		}
	}

	private void OnDrawGizmosSelected() {
		if(collider2D != null) {
			Gizmos.color = Color.red;
			Vector2 bottomCenter = new Vector2(collider2D.bounds.center.x, collider2D.bounds.min.y);
			Gizmos.DrawWireCube(bottomCenter, new Vector2(collider2D.size.x, groundCheckBoxHeight));
		}
	}

	public void BirthSlimeBaby() {
		GameObject.Instantiate(slimeChunkPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
		HandleMassLost(1f, MassChangeSourceType.BabyChunk);
	}

	public void Die() {
		if(!isDead) {
			isDead = true;
			GameManager.PlayerDied();
			rb2D.velocity = Vector3.zero;
			rb2D.isKinematic = true;
			PlayOnce(dieSound);
			Destroy(gameObject, 0.5f);
			
		}
	}

	void PlaySound(AudioClip sound) {
		playerSound.clip = sound;
		if(!playerSound.isPlaying) {
			playerSound.Play();
		}
	}

	void PlayOnce(AudioClip sound) {
		playerSound.PlayOneShot(sound);
	}
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeBaby : MonoBehaviour {
	[SerializeField] private Sprite slimePileSprite;
	[SerializeField] private float initialUpwardVelocity = 1f;

	private SpriteRenderer sr;
	private Rigidbody2D rb;

	private void Awake() {
		sr = GetComponent<SpriteRenderer>();
		rb = GetComponent<Rigidbody2D>();
	}

	private void Start() {
		rb.velocity = Vector2.up * initialUpwardVelocity;
	}

	private void OnCollisionEnter2D(Collision2D other) {
		sr.sprite = slimePileSprite;	
	}
}

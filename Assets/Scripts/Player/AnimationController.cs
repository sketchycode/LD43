using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour {
	private Controller playerController;
	private Animator animator;
	private SpriteRenderer spriteRenderer;

	private void Start() {
		playerController = GetComponent<Controller>();
		animator = GetComponent<Animator>();
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	private void Update() {
		animator.SetBool("isMoving", playerController.IsMoving);
		animator.SetBool("isGrounded", playerController.IsGrounded);
		spriteRenderer.flipX = !playerController.IsMovingRight;
	}
}

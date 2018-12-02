using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimePatch : MonoBehaviour {
	[SerializeField] private float maxLifetime;
	[SerializeField] Animator animator;

	private float currentLifetime = 0;
	private bool isDissolving = false;
	private Controller player;

	public Action<SlimePatch> DissolveStarting = delegate { };
	public Action<SlimePatch> Dissolved = delegate { };

	public bool IsDissolving => isDissolving;

	public int PositionHash { get; set; }

	private void Awake() {
		animator = GetComponent<Animator>();
	}

	private void Update() {
		if(!isDissolving) {
			currentLifetime += Time.deltaTime;
			if(currentLifetime > maxLifetime) {

				DissolveStarting(this);
				isDissolving = true;
				animator.SetBool("isDissolving", true);
			}
		}
	}

	public void Reapply() {
		currentLifetime = 0;
		isDissolving = false;
		animator.SetBool("isDissolving", false);
	}

	public void OnDissolveAnimCompleted() {
		Dissolved(this);
		if(player) { player.OccupiedSlimes.Remove(this); }
		GameObject.Destroy(gameObject, 0.1f);
	}

	private void OnTriggerEnter2D(Collider2D other) {
		var player = other.GetComponent<Controller>();
		if(player != null) {
			this.player = player;
			player.OccupiedSlimes.Add(this);
		}
	}

	private void OnTriggerExit2D(Collider2D other) {
		var player = other.GetComponent<Controller>();
		if(player != null) {
			player.OccupiedSlimes.Remove(this);
		}		
	}
}

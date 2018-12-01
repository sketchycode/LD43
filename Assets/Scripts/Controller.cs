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
	
	private Rigidbody2D rb2D;
	[SerializeField] private float mass;

	void Start() {
		rb2D = GetComponent<Rigidbody2D>();
		mass = maxMass;
	}

	void Update () {
		var speed = Mathf.Lerp(maxSpeed, minSpeed, Mathf.InverseLerp(minMass, maxMass, mass));
		float velY = rb2D.velocity.y;
		rb2D.velocity = Vector2.zero;
		if(Input.GetKey(KeyCode.A)) {
			rb2D.velocity += Vector2.left * speed;
		}
		if(Input.GetKey(KeyCode.D)) {
			rb2D.velocity += Vector2.right * speed;
		}
		rb2D.velocity += Vector2.up * velY;
		mass -= rb2D.velocity.magnitude * massLossWhileMoving * Time.deltaTime;
	}
}

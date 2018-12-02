using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour {
	public BackgroundLayer[] Layers;

	private Vector3 startPosition;

	void Start() {
		startPosition = transform.position;
	}

	void Update() {
		ApplyOffset();
	}

	float GetHorizontalOffset() {
		return transform.position.x - startPosition.x;
	}

	float GetVerticalOffset() {
		return transform.position.y - startPosition.y;

	}

	void ApplyOffset() {
		float horizontal = GetHorizontalOffset();
		float vertical = GetVerticalOffset();

		foreach(BackgroundLayer layer in Layers) {
			layer.Offset(horizontal, vertical);
		}

		startPosition = transform.position;
	}
}

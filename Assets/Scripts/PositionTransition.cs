using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionTransition : Interactable {
	public Vector3 startPosition;
	public Vector3 endPosition;
	public bool shouldMoveToEnd;
	
	public float moveTimer = 0.0f;

	public void Activate() {
		shouldMoveToEnd = !shouldMoveToEnd;
	}

	void Update () {
		if(shouldMoveToEnd) {
			moveTimer += Time.deltaTime;
		} else {
			moveTimer -= Time.deltaTime;
		}
		moveTimer = Mathf.Clamp(moveTimer, 0, 1);
		
		Move();
	}

	void Move() {
		transform.localPosition = Vector3.Lerp(startPosition, endPosition, moveTimer);
	}
	
	void OnDrawGizmos() {
		Vector3 startOffset = transform.position + startPosition;
		Vector3 endOffset = transform.position + endPosition;

		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(endOffset, Vector3.one);
		
		Gizmos.color = Color.yellow;
		Gizmos.DrawLine(startOffset, endOffset);

		Gizmos.color = Color.green;
		Gizmos.DrawWireCube(startOffset, Vector3.one);
	}

    public override void Enable()
    {
        shouldMoveToEnd = true;
    }

    public override void Disable()
    {
        shouldMoveToEnd = false;
    }
}

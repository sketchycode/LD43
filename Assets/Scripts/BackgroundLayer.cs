using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BackgroundLayer {
	public MeshRenderer render;
	public float horizontalShift;
	public float verticalShift;
	
	public void Offset(float horizontal, float vertical) {
		render.material.mainTextureOffset += new Vector2(horizontal * horizontalShift, vertical * verticalShift);
	}
}

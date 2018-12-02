using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureScroll : MonoBehaviour {
	public MeshRenderer render;
	public Vector2 constantOffset;
	
	void Update() {
		render.material.mainTextureOffset += (constantOffset * Time.deltaTime);
	}
}

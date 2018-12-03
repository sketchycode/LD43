using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class FadeControl {
	public Image fadePanel;
	public float fadeSpeed;

	private Color black = Color.black;
	private Color clear = Color.clear;

	private Color placeholderColor;

	public IEnumerator FadeIn() {
		for(float timer = 0.0f; timer <= 1; timer += Time.deltaTime * fadeSpeed) {
			fadePanel.color = Color.Lerp(black, clear, timer);
			yield return null;
		}
	}

	public IEnumerator FadeOut() {
		for(float timer = 0.0f; timer <= 1; timer += Time.deltaTime * fadeSpeed) {
			fadePanel.color = Color.Lerp(clear, black, timer);
			yield return null;
		}
	}
}

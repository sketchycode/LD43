using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerWinPoint : MonoBehaviour {
	public string nextLevelName;
	public Animator anim;

	private bool didWin = false;

	void Start() {
		anim.StartPlayback(); //steals control of animations from animator
	}

	IEnumerator Win() {
		anim.StopPlayback(); //restores control of animations from animator
		for(float timer = 0; timer < 1.0f; timer += Time.deltaTime) {
			yield return null;	
		}

		loadNextLevel();
	}

	void loadNextLevel() {
		Debug.Log("Loading next level: " + nextLevelName);
		SceneManager.LoadScene(nextLevelName);
	}

	void OnCollisionEnter2D(Collision2D collision)
    {
		Debug.Log(collision.gameObject);
		if(collision.gameObject.GetComponent<Controller>()) {
			Debug.Log("foundPlayer");
			if(!didWin) {
				StartCoroutine(Win());
				Destroy(collision.gameObject);
			}
		}
    }
}

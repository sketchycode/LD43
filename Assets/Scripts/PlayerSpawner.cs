using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour {
	public GameObject player;
	public Animator anim;
	public Transform spawnLocation;
	// Use this for initialization
	void Start () {
		StartCoroutine(Spawn());
	}
	
	IEnumerator Spawn() {
		anim.SetBool("doSpawn", true);
		for(float timer = 0; timer < 1.0f; timer += Time.deltaTime) {
			yield return null;	
		}
		anim.SetBool("doSpawn", false);
		GameObject newPlayer = GameObject.Instantiate(player, spawnLocation.position, Quaternion.identity);
		GameManager.Instance.SetPlayer(newPlayer);
	}
}

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
	public static Action PlayerDied = delegate { };
    public static Action OpenPauseMenu = delegate { };
    public static GameManager Instance;
    private GameObject player;
    private CameraController sceneCamera;

    [SerializeField] private SlimePatch slimePatchPrefab;

    private Dictionary<int, SlimePatch> slimeTracker = new Dictionary<int, SlimePatch>();

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        PlayerDied += OnPlayerDied;
    }

    private void OnPlayerDied() {
        // reset scene? wait for ok?
        Debug.Log("got signal for player death");
    }

    public void PlaceSlimeAtTilePosition(Vector2Int pos) {
        Debug.Log("Slime time");
        Debug.DrawLine(new Vector2(pos.x + 0.2f, pos.y), new Vector2(pos.x - 0.2f, pos.y), Color.green);
        Debug.DrawLine(new Vector2(pos.x, pos.y + 0.2f), new Vector2(pos.x, pos.y - 0.2f), Color.green);
        var positionHash = pos.x * 10000 + pos.y;
        if(slimeTracker.ContainsKey(positionHash)) {
            slimeTracker[positionHash].Reapply();
            return;
        }
        var slime = GameObject.Instantiate(slimePatchPrefab, new Vector2(pos.x + 0.5f, pos.y + .5f), Quaternion.identity);
        slime.PositionHash = positionHash;
        slimeTracker.Add(slime.PositionHash, slime);
        slime.Dissolved += OnSlimeDissolved;
    }

    private void OnSlimeDissolved(SlimePatch slime) {
        if(slimeTracker.ContainsKey(slime.PositionHash)) {
            slimeTracker.Remove(slime.PositionHash);
        }
    }

    public void LoadLevel(string levelName) {
        ResetSceneParameters();
        SceneManager.LoadScene(levelName);
    }

    public void ResetLevel() {
        ResetSceneParameters();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void ResetSceneParameters() {
        player = null;
        sceneCamera = null;
    }

    public void SetPlayer(GameObject player) {
        this.player = player;
        if(sceneCamera) {
            sceneCamera.SetTarget(player.transform);
        }
    }

    public void SetCamera(CameraController camera) {
        this.sceneCamera = camera;
    }
}
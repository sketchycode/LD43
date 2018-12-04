using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
	public static Action PlayerDied = delegate { };
    public static Action OpenPauseMenu = delegate { };
    public static GameManager Instance;
    private GameObject player;
    private Controller playerController;
    private CameraController sceneCamera;

    [SerializeField] private SlimePatch slimePatchPrefab;
    [SerializeField] private Canvas levelUIPrefab;

    private Dictionary<int, SlimePatch> slimeTracker = new Dictionary<int, SlimePatch>();
    private Slider blobBar;

    private void Awake() {
        Instance = this;
        var ui = GameObject.Instantiate(levelUIPrefab);
        blobBar = ui.GetComponentInChildren<Slider>();
    }

    private void Start() {
        PlayerDied += OnPlayerDied;
    }

    private void Update() {
    }

    void OnDestroy() {
        PlayerDied -= OnPlayerDied;
    }

    private void OnPlayerDied() {
        // reset scene? wait for ok?
        StartCoroutine(ResetScene());
        Debug.Log("got signal for player death");
    }

    public IEnumerator ResetScene() {
        StartCoroutine(sceneCamera.fade.FadeOut());
        for(float timer = 0; timer < 1.5f; timer += Time.deltaTime) {
            yield return null;
        }

        StopAllCoroutines();
        ResetLevel();
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
        if(playerController) {
            playerController.MassLost -= OnMassLost;
            playerController = null;
        }
    }

    public void SetPlayer(GameObject player) {
        this.player = player;
        if(sceneCamera) {
            sceneCamera.SetTarget(player.transform);
        }
        playerController = player.GetComponent<Controller>();
        if(playerController) {
            playerController.MassLost += OnMassLost;
        }
    }

    public void SetCamera(CameraController camera) {
        this.sceneCamera = camera;
    }

    private void OnMassLost(MassChangeEvent e) {
        blobBar.value = e.NewMassNormalized;
    }
}
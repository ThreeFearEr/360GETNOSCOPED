using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour {

    Texture2D iddleCursor;
    Texture2D reloadCursor;

    CurtainFader curtainFader;
    public PlayerRadial Radial;
    public PlayerReloader Reloader;

    TextMeshProUGUI scoreTxtBox;
    TextMeshProUGUI additionTxtBox;
    Animator scoreAnimator;

    float additionTime;
    int curAddition;

    private void Awake() {
        GameManager.UIManager = this;
        GameManager.Reset();

        curtainFader = GetComponentInChildren<CurtainFader>();
        Radial = GetComponentInChildren<PlayerRadial>();
        Reloader = GetComponentInChildren<PlayerReloader>();
        scoreTxtBox = GetComponentsInChildren<TextMeshProUGUI>(true)[0];
        additionTxtBox = GetComponentsInChildren<TextMeshProUGUI>(true)[1];
        scoreAnimator = GetComponentInChildren<Animator>();

        iddleCursor = Resources.Load<Texture2D>("Cursors/hitmarker");
        reloadCursor = Resources.Load<Texture2D>("Cursors/hourglass");
        SetCursorToIddle();
    }

    public void UpdateScore(int value) {
        if(Time.time - additionTime < 1.2f) {
            curAddition += value;
        }
        else {
            curAddition = value;
        }

        additionTime = Time.time;
        additionTxtBox.text = "+" + curAddition.ToString();
        scoreTxtBox.text = GameManager.Score.ToString();
        scoreAnimator.Play("ScoreAdd");
    }

    public void SetCursorToIddle() {
        Vector2 hotspot = new Vector2(iddleCursor.width / 2, iddleCursor.height / 2);
        Cursor.SetCursor(iddleCursor, hotspot, CursorMode.Auto);
    }
    public void SetCursorToReload() {
        Vector2 hotspot = new Vector2(reloadCursor.width / 2, reloadCursor.height / 2);
        Cursor.SetCursor(reloadCursor, hotspot, CursorMode.Auto);
    }

    private void Start() {
        curtainFader.FadeOut();
    }

    bool resetEnabled = false;
    private void Update() {
        if(Input.GetKeyDown(KeyCode.Escape) || (Input.GetKeyDown(KeyCode.Mouse0) && !GameManager.isPlaying && resetEnabled)) {
            curtainFader.LoadGameAsync("Game");
        }
    }

    public void Die() {
        StartCoroutine(waitBeforeScoreDie());
    }
    IEnumerator waitBeforeScoreDie() {
        scoreAnimator.Play("ScoreDie");
        yield return new WaitForSeconds(2);
        curtainFader.FadeIn();
        resetEnabled = true;
    }
}

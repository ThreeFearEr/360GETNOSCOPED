using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour {

    Texture2D iddleCursor;
    Texture2D reloadCursor;

    public CurtainFader curtainFader;
    public PlayerRadial Radial;
    public PlayerReloader Reloader;

    TMP_InputField nicknameInputField;
    TextMeshProUGUI scoreTxtBox;
    TextMeshProUGUI additionTxtBox;
    TextMeshProUGUI highscoreBox;
    Animator scoreAnimator;

    float additionTime;
    int curAddition;

    private void Awake() {
        GameManager.UIController = this;

        curtainFader = GetComponentInChildren<CurtainFader>();
        Radial = GetComponentInChildren<PlayerRadial>();
        Reloader = GetComponentInChildren<PlayerReloader>();
        nicknameInputField = GetComponentsInChildren<TMP_InputField>(true)[0];
        scoreTxtBox = GetComponentsInChildren<TextMeshProUGUI>(true)[1];
        highscoreBox = GetComponentsInChildren<TextMeshProUGUI>(true)[2];
        additionTxtBox = GetComponentsInChildren<TextMeshProUGUI>(true)[3];
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
    public void UpdateHighscore(int value) {
        highscoreBox.text = value.ToString();
    }

    public void SetCursorToIddle() {
        Vector2 hotspot = new Vector2(iddleCursor.width / 2, iddleCursor.height / 2);
        Cursor.SetCursor(iddleCursor, hotspot, CursorMode.Auto);
    }
    public void SetCursorToReload() {
        Vector2 hotspot = new Vector2(reloadCursor.width / 2, reloadCursor.height / 2);
        Cursor.SetCursor(reloadCursor, hotspot, CursorMode.Auto);
    }

    public void ChangeNickname() {
        PlayerPrefs.SetString("Nickname", nicknameInputField.text);
        GameManager.nickname = nicknameInputField.text;
    }
    public void UpdateNickname() {
        nicknameInputField.text = GameManager.nickname;
        nicknameInputField.caretPosition = 10;
    }

    private void Start() {
        curtainFader.FadeOut();
    }

    bool resetEnabled = false;
    private void Update() {
        if(Input.GetKeyDown(KeyCode.Escape) || (Input.GetKeyDown(KeyCode.Space) && !GameManager.isPlaying && resetEnabled)) {
            GameManager.WebGate.SetHighscore();
            curtainFader.NextScene();
        }
    }

    public void Die() {
        StartCoroutine(waitBeforeScoreDie());
    }
    IEnumerator waitBeforeScoreDie() {
        StartCoroutine(GameManager.AudioController.FadeOut());
        scoreAnimator.Play("ScoreDie");
        yield return new WaitForSeconds(2);
        curtainFader.FadeIn();
        resetEnabled = true;
    }
}

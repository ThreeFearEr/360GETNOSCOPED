using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class IntroVideo : MonoBehaviour {

    VideoPlayer videoPlayer;
    CanvasGroup canvasGroup;

    public float fadeDuration;

    void Awake() {
        videoPlayer = GetComponentInChildren<VideoPlayer>(true);
        canvasGroup = GetComponent<CanvasGroup>();
        
    }

    private void Start() {
        if(GameManager.Intro == false) {
            GameManager.Reset();
            return;
        }
        Debug.Log(videoPlayer.gameObject.activeSelf);
        videoPlayer.gameObject.SetActive(true);
        Debug.Log(videoPlayer.gameObject.activeSelf);
        videoPlayer.Play();
        StartCoroutine(CheckVideoFinish());
    }

    private void Update() {
        Debug.Log(videoPlayer.gameObject.activeSelf);

    }

    private IEnumerator CheckVideoFinish() {
        for (float f = 0; f < videoPlayer.length / videoPlayer.playbackSpeed; f += Time.deltaTime) {
            if(Input.GetKeyDown(KeyCode.Space)) break;
            yield return null;
        }
        GameManager.Reset();

        yield return StartCoroutine(FadeVideo(canvasGroup, 1, 0));
    }

    private IEnumerator FadeVideo(CanvasGroup cg, float startAlpha, float endAlpha) {
        if(canvasGroup.alpha == endAlpha) yield break;
        canvasGroup.gameObject.SetActive(true);
        float elapsedTime = 0f;
        cg.alpha = startAlpha;

        // Loop until we reach the desired alpha
        while(elapsedTime < fadeDuration) {
            elapsedTime += Time.deltaTime;
            cg.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
            yield return null;
        }
        cg.alpha = endAlpha;
        Debug.Log("setFalse");
        if(endAlpha == 0) canvasGroup.gameObject.SetActive(false);
    }

}

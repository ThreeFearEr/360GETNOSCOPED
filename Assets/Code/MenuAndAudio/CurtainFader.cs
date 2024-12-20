using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CurtainFader : MonoBehaviour
{
    private Image curtain;
    public CanvasGroup canvasGroup;  // Reference to the CanvasGroup component
    public float fadeDuration = 0.4f;  // Duration for fading in/out

    private void Awake() {
        canvasGroup = GetComponent<CanvasGroup>();
        curtain = GetComponentInChildren<Image>(true);
    }

    // Start fading out
    public void FadeOut() {
        StartCoroutine(FadeCanvasGroup(canvasGroup, 1f, 0f));
    }

    // Start fading in
    public void FadeIn() {
        StartCoroutine(FadeCanvasGroup(canvasGroup, 0f, 1f));
    }

    // Coroutine to handle the fade effect
    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float startAlpha, float endAlpha) {
        if(canvasGroup.alpha == endAlpha) yield break;
        curtain.gameObject.SetActive(true);
        float elapsedTime = 0f;
        cg.alpha = startAlpha;

        // Loop until we reach the desired alpha
        while(elapsedTime < fadeDuration) {
            elapsedTime += Time.deltaTime;
            cg.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
            yield return null;
        }
        cg.alpha = endAlpha;

        if(endAlpha == 0) curtain.gameObject.SetActive(false);
    }

    public void LoadGameAsync(string sceneName) {
        StartCoroutine(LoadSceneCoroutine(sceneName));
    }

    private IEnumerator LoadSceneCoroutine(string sceneName) {
        yield return StartCoroutine(FadeCanvasGroup(canvasGroup, 0f, 1f));

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
        while(!asyncOperation.isDone) {
            yield return null;
        }
    }
}

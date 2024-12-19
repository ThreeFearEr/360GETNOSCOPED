using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class MenuVideo : MonoBehaviour {

    VideoPlayer videoPlayer;
    CurtainFader curtainFader;

    void Awake() {
        GetComponentInChildren<RawImage>(true).gameObject.SetActive(true);
        videoPlayer = GetComponentInChildren<VideoPlayer>();
        curtainFader = transform.parent.GetComponentInChildren<CurtainFader>();

        videoPlayer.Play();
        StartCoroutine(CheckVideoFinish());
    }

    private IEnumerator CheckVideoFinish() {
        for (float f = 0; f < videoPlayer.length / 1.4f; f += Time.deltaTime) {
            if(Input.GetKeyDown(KeyCode.Space)) break;
            yield return null;
        }
        curtainFader.LoadGameAsync("Game");
    }

}

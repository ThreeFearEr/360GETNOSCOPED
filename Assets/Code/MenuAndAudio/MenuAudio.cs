using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuAudio : MonoBehaviour {

    public AudioSource audioSource;  // Reference to the AudioSource component
    public float fadeDuration = 1f;  // Duration of fade-out

    void Awake() {
        audioSource = GetComponent<AudioSource>();
    }

    public IEnumerator FadeOut() {
        float startVolume = audioSource.volume;
        
        while(audioSource.volume > 0) {
            audioSource.volume -= startVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }

        audioSource.Stop();  // Stop the audio
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAudio : MonoBehaviour {

    public AudioClip startSound;  // The song to play first
    public AudioClip loopSound; // The song to loop after the first one

    private AudioSource audioSource;  // Reference to the AudioSource component
    public float fadeDuration = 0.4f;  // Duration of the fade-in
    float maxVolume = 0.2f;

    void Awake() {
        audioSource = GetComponent<AudioSource>();
        
        audioSource.clip = loopSound;
        audioSource.Play();
        audioSource.Pause();
    }

    void Start() {
        audioSource.clip = startSound;
        audioSource.loop = false;
        audioSource.Play();

        StartCoroutine(PlayFirstTrackWithFadeIn());
        StartCoroutine(WaitForSongToEnd());
    }

    IEnumerator WaitForSongToEnd() {
        while(audioSource.isPlaying) {
            yield return null; // Check every frame
        }
        audioSource.clip = loopSound;
        audioSource.loop = true;
        audioSource.Play();
    }

    private IEnumerator PlayFirstTrackWithFadeIn() {
        audioSource.volume = 0;  // Start with volume at 0
        audioSource.Play();  // Play the first track

        // Fade in the audio
        while(audioSource.volume < maxVolume) {
            audioSource.volume += (maxVolume / fadeDuration) * Time.deltaTime;  // Gradually increase volume
            yield return null;
        }

        audioSource.volume = 0.2f;  // Ensure volume is at max
    }

    public IEnumerator FadeOut() {
        float startVolume = audioSource.volume;
        // Gradually reduce volume to 0
        while(audioSource.volume > 0) {
            audioSource.volume -= (startVolume * Time.deltaTime) / fadeDuration;
            yield return null;
        }
        audioSource.Stop();  // Stop the audio
    }

}

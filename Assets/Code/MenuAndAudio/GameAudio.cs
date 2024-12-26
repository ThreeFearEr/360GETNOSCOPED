using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class GameAudio : MonoBehaviour {

    public AudioClip startSound;  // The song to play first
    public AudioClip loopSound; // The song to loop after the first one

    private AudioSource audioSource;  // Reference to the AudioSource component
    public float fadeDuration = 0.4f;  // Duration of the fade-in
    float maxVolume = 0.2f;
    
    Dictionary<string, AudioClip> soundEffects = new Dictionary<string, AudioClip>();
    List<AudioSource> soundSources = new List<AudioSource>();
    private AudioSource GetAvailableSource() {
        foreach(AudioSource source in soundSources) {
            if(!source.isPlaying) return source;
        }

        AudioSource newSource = gameObject.AddComponent<AudioSource>();
        newSource.playOnAwake = false;
        soundSources.Add(newSource);
        return newSource;
    }
    public void PlayAudio(string key) {
        PlayAudio(key, Vector3.zero);
    }
    public void PlayAudio(string key, Vector3 position) {
        if(!soundEffects.ContainsKey(key)) {
            Debug.LogWarning("Source not found [" +  key + "]");
            return;
        }
        AudioSource source = GetAvailableSource();
        //if(source.clip != soundEffects[key])
        source.clip = soundEffects[key];
        source.transform.position = position; // Set position for 3D sound
        source.spatialBlend = 0; // 0 = 2D, 1 = 3D
        source.Play();
    }

    void Awake() {
        GameManager.AudioController = this;
        audioSource = GetComponent<AudioSource>();
        
        foreach(AudioClip soundEffect in Resources.LoadAll<AudioClip>("Audio/")) {
            soundEffects.Add(soundEffect.name, soundEffect);
            audioSource.clip = soundEffect;
            audioSource.Play();
            audioSource.Pause();
        }
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
